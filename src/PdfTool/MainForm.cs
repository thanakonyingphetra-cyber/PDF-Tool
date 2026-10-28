using System.Drawing;
using System.Windows.Forms;

namespace PdfTool;

internal sealed class MainForm : Form
{
    private readonly ListBox _files = new()
    {
        Dock = DockStyle.Fill,
        SelectionMode = SelectionMode.MultiExtended,
        HorizontalScrollbar = true,
        IntegralHeight = false
    };

    private readonly Label _status = new()
    {
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        Text = "พร้อมใช้งาน • ประมวลผล PDF ภายในเครื่อง",
        AutoEllipsis = true
    };

    private readonly List<Button> _buttons = new();
    private bool _busy;

    public MainForm()
    {
        Text = "PDF Tool 0.1 • Portable";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(650, 430);
        Size = new Size(850, 550);
        Font = new Font("Segoe UI", 10);
        AllowDrop = true;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(14),
            RowCount = 4,
            ColumnCount = 1
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

        var title = new Label
        {
            Text = "PDF Tool  |  Portable • Offline • No installation",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        root.Controls.Add(title, 0, 0);

        var management = MakeButtonBar();
        AddButton(management, "เพิ่ม PDF", (_, _) => SelectFiles());
        AddButton(management, "ลบจากรายการ", (_, _) => RemoveSelected());
        AddButton(management, "ขึ้น", (_, _) => MoveSelected(-1));
        AddButton(management, "ลง", (_, _) => MoveSelected(1));
        AddButton(management, "ล้างรายการ", (_, _) => _files.Items.Clear());
        root.Controls.Add(management, 0, 1);

        root.Controls.Add(_files, 0, 2);

        var footer = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
        footer.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));
        footer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var tools = MakeButtonBar();
        AddButton(tools, "รวม PDF ตามลำดับรายการ", async (_, _) => await MergeAsync());
        AddButton(tools, "แยกทุกหน้า PDF", async (_, _) => await SplitAsync());
        footer.Controls.Add(tools, 0, 0);
        footer.Controls.Add(_status, 0, 1);
        root.Controls.Add(footer, 0, 3);

        Controls.Add(root);
        DragEnter += OnDragEnter;
        DragDrop += OnDragDrop;
        _files.DragEnter += OnDragEnter;
        _files.DragDrop += OnDragDrop;
        _files.AllowDrop = true;
    }

    private static FlowLayoutPanel MakeButtonBar() => new()
    {
        Dock = DockStyle.Fill,
        FlowDirection = FlowDirection.LeftToRight,
        WrapContents = true,
        AutoScroll = true
    };

    private void AddButton(FlowLayoutPanel parent, string text, EventHandler handler)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(9, 4, 9, 4),
            Margin = new Padding(2, 3, 8, 3)
        };
        button.Click += handler;
        parent.Controls.Add(button);
        _buttons.Add(button);
    }

    private void SelectFiles()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "เลือกไฟล์ PDF",
            Filter = "PDF files (*.pdf)|*.pdf",
            Multiselect = true,
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            AddFiles(dialog.FileNames);
    }

    private void AddFiles(IEnumerable<string> paths)
    {
        foreach (string path in paths)
        {
            if (!File.Exists(path) || !Path.GetExtension(path).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                continue;
            string full = Path.GetFullPath(path);
            if (!_files.Items.Cast<string>().Contains(full, StringComparer.OrdinalIgnoreCase))
                _files.Items.Add(full);
        }
        _status.Text = _files.Items.Count + " ไฟล์ในรายการ • ลาก PDF มาวางได้";
    }

    private void RemoveSelected()
    {
        for (int i = _files.SelectedIndices.Count - 1; i >= 0; i--)
            _files.Items.RemoveAt(_files.SelectedIndices[i]);
    }

    private void MoveSelected(int direction)
    {
        if (_files.SelectedIndices.Count != 1)
        {
            MessageBox.Show(this, "เลือกไฟล์หนึ่งรายการก่อนเลื่อนลำดับ", "PDF Tool");
            return;
        }

        int oldIndex = _files.SelectedIndex;
        int newIndex = oldIndex + direction;
        if (newIndex < 0 || newIndex >= _files.Items.Count)
            return;

        var item = _files.Items[oldIndex]!;
        _files.Items.RemoveAt(oldIndex);
        _files.Items.Insert(newIndex, item);
        _files.SelectedIndex = newIndex;
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = !_busy && e.Data?.GetDataPresent(DataFormats.FileDrop) == true
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void OnDragDrop(object? sender, DragEventArgs e)
    {
        if (!_busy && e.Data?.GetData(DataFormats.FileDrop) is string[] paths)
            AddFiles(paths);
    }

    private async Task MergeAsync()
    {
        var paths = _files.Items.Cast<string>().ToArray();
        if (paths.Length < 2)
        {
            MessageBox.Show(this, "กรุณาเพิ่ม PDF อย่างน้อย 2 ไฟล์", "PDF Tool");
            return;
        }

        await RunAsync(() => "รวม PDF สำเร็จ:\n" + PdfOperations.Merge(paths));
    }

    private async Task SplitAsync()
    {
        var paths = _files.Items.Cast<string>().ToArray();
        if (paths.Length == 0)
        {
            MessageBox.Show(this, "กรุณาเพิ่ม PDF ก่อน", "PDF Tool");
            return;
        }

        await RunAsync(() =>
        {
            int count = 0;
            foreach (string path in paths)
                count += PdfOperations.Split(path);
            return "แยก PDF สำเร็จ " + count + " หน้า";
        });
    }

    private async Task RunAsync(Func<string> work)
    {
        if (_busy)
            return;

        _busy = true;
        foreach (var button in _buttons)
            button.Enabled = false;
        _status.Text = "กำลังประมวลผลในเครื่อง...";

        try
        {
            string result = await Task.Run(work);
            _status.Text = "เสร็จสิ้น • ไม่แก้ไขไฟล์ต้นฉบับ";
            MessageBox.Show(this, result, "PDF Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _status.Text = "เกิดข้อผิดพลาด • ตรวจสอบสิทธิ์ไฟล์และรูปแบบ PDF";
            MessageBox.Show(this,
                "ไม่สามารถประมวลผลได้:\n" + ex.Message +
                "\n\nไฟล์ต้นฉบับไม่ได้ถูกแก้ไข หากแยกหลายหน้าอาจมีไฟล์ผลลัพธ์บางส่วน",
                "PDF Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _busy = false;
            foreach (var button in _buttons)
                button.Enabled = true;
        }
    }
}
