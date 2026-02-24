using Antlr4.Runtime;
using AntlrCSharpLib;

namespace HalsteadGui;

public class MainForm : Form
{
    private readonly Color _formBackColor = Color.FromArgb(240, 242, 245);
    private readonly Color _cardBackColor = Color.White;
    private readonly Color _accentColor = Color.FromArgb(52, 58, 64);
    private readonly Color _accentHoverColor = Color.FromArgb(73, 80, 87);
    private readonly Color _textColorDark = Color.FromArgb(33, 37, 41);
    private readonly Color _textColorLight = Color.FromArgb(108, 117, 125);
    private readonly Color _borderColor = Color.FromArgb(233, 236, 239);

    private readonly Font _fontUIBold = new Font("Tahoma", 11, FontStyle.Bold);
    private readonly Font _fontUINormal = new Font("Tahoma", 10);
    private readonly Font _fontCode = new Font("Lucida Console", 11);
    private readonly Font _fontFormula = new Font("Tahoma", 11);

    private TextBox txtCode;
    private Button btnCalculate;
    private DataGridView dgvMainMetrics;
    private Label lblFormulas;
    private SplitContainer splitContainer;

    public MainForm()
    {
        Text = "Метрология - Метрики Холстеда";
        Width = 1200;
        Height = 780;
        MinimumSize = new Size(1000, 680);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = _formBackColor;

        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);

        BuildUI();

        Load += (s, e) =>
        {
            ApplySplitRatio();
        };

        Resize += (s, e) =>
        {
            ApplySplitRatio();
        };
    }

    private void ApplySplitRatio()
    {
        int leftWidth = (int)(this.ClientSize.Width * 0.45);
        splitContainer.SplitterDistance = leftWidth;
    }

    private void BuildUI()
    {
        splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 350,
            SplitterWidth = 8,
            BackColor = _formBackColor,
            Margin = new Padding(15)
        };

        var panelLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

        var codeCard = CreateCardPanel();
        codeCard.Dock = DockStyle.Fill;

        var lblCodeHeader = CreateHeaderLabel("Исходный код программы");

        txtCode = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Font = _fontCode,
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            BackColor = _cardBackColor,
            ForeColor = _textColorDark,
            Text = "let x = 10\nlet y = 20\nlet n = vs * 2\nif x < y {\n    print(abs(x))\n}"
        };

        var txtWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10), BackColor = _cardBackColor };
        txtWrapper.Controls.Add(txtCode);

        codeCard.Controls.Add(lblCodeHeader);
        codeCard.Controls.Add(txtWrapper);
        txtWrapper.BringToFront();

        btnCalculate = new Button
        {
            Text = "РАССЧИТАТЬ МЕТРИКИ",
            Dock = DockStyle.Bottom,
            Height = 50,
            FlatStyle = FlatStyle.Flat,
            BackColor = _accentColor,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand,
            TabStop = false
        };
        btnCalculate.FlatAppearance.BorderSize = 0;
        btnCalculate.MouseEnter += (s, e) => btnCalculate.BackColor = _accentHoverColor;
        btnCalculate.MouseLeave += (s, e) => btnCalculate.BackColor = _accentColor;
        btnCalculate.Click += BtnCalculate_Click;

        var spacerBtn = new Panel { Dock = DockStyle.Bottom, Height = 15, BackColor = _formBackColor };

        panelLeft.Controls.Add(codeCard);
        panelLeft.Controls.Add(spacerBtn);
        panelLeft.Controls.Add(btnCalculate);

        var panelRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5, 15, 15, 15) };

        var cardTable = CreateCardPanel();
        cardTable.Dock = DockStyle.Fill;

        var lblTableHeader = CreateHeaderLabel("Расчет базовых метрик Холстеда");

        dgvMainMetrics = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            BackgroundColor = _cardBackColor,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.Single,
            GridColor = Color.FromArgb(200, 200, 200),
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            Padding = new Padding(5)
        };

        dgvMainMetrics.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        dgvMainMetrics.ColumnHeadersDefaultCellStyle.ForeColor = _textColorDark;
        dgvMainMetrics.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgvMainMetrics.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dgvMainMetrics.ColumnHeadersHeight = 40;
        dgvMainMetrics.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

        dgvMainMetrics.DefaultCellStyle.BackColor = _cardBackColor;
        dgvMainMetrics.DefaultCellStyle.ForeColor = _textColorDark;
        dgvMainMetrics.DefaultCellStyle.Font = _fontUINormal;
        dgvMainMetrics.DefaultCellStyle.SelectionBackColor = _cardBackColor;
        dgvMainMetrics.DefaultCellStyle.SelectionForeColor = _textColorDark;
        dgvMainMetrics.RowTemplate.Height = 30;

        var colJ = dgvMainMetrics.Columns.Add("j", "j");
        var colOp = dgvMainMetrics.Columns.Add("Operator", "Оператор");
        var colF1 = dgvMainMetrics.Columns.Add("f1j", "f1j");
        var colI = dgvMainMetrics.Columns.Add("i", "i");

        dgvMainMetrics.CellPainting += (s, e) =>
        {
            if (e.ColumnIndex == colF1)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                using (var pen = new Pen(Color.FromArgb(120, 120, 120), 2))
                {
                    int x = e.CellBounds.Right - 1;
                    e.Graphics.DrawLine(pen, x, e.CellBounds.Top, x, e.CellBounds.Bottom);
                }

                e.Handled = true;
            }
        };


        var colOpr = dgvMainMetrics.Columns.Add("Operand", "Операнд");
        var colF2 = dgvMainMetrics.Columns.Add("f2i", "f2i");

        foreach (DataGridViewColumn col in dgvMainMetrics.Columns)
        {
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        dgvMainMetrics.Columns[colJ].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dgvMainMetrics.Columns[colJ].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        dgvMainMetrics.Columns[colOp].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dgvMainMetrics.Columns[colOp].DefaultCellStyle.Font = _fontCode;

        dgvMainMetrics.Columns[colF1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dgvMainMetrics.Columns[colF1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        dgvMainMetrics.Columns[colI].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dgvMainMetrics.Columns[colI].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        dgvMainMetrics.Columns[colOpr].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dgvMainMetrics.Columns[colOpr].DefaultCellStyle.Font = _fontCode;

        dgvMainMetrics.Columns[colF2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        dgvMainMetrics.Columns[colF2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        cardTable.Controls.Add(lblTableHeader);
        cardTable.Controls.Add(dgvMainMetrics);
        dgvMainMetrics.BringToFront();

        var cardFormulas = CreateCardPanel();
        cardFormulas.Dock = DockStyle.Bottom;
        cardFormulas.Height = 200;

        var lblFormulasHeader = CreateHeaderLabel("Расширенные метрики");

        lblFormulas = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = _fontFormula,
            ForeColor = _textColorDark,
            Text = "Ожидание данных...",
            TextAlign = ContentAlignment.BottomLeft,
            Padding = new Padding(20, 0, 10, 20)
        };

        cardFormulas.Controls.Add(lblFormulasHeader);
        cardFormulas.Controls.Add(lblFormulas);
        lblFormulas.BringToFront();

        var spacerMiddle = new Panel { Dock = DockStyle.Bottom, Height = 15, BackColor = _formBackColor };

        panelRight.Controls.Add(cardTable);
        panelRight.Controls.Add(spacerMiddle);
        panelRight.Controls.Add(cardFormulas);
        cardTable.BringToFront();

        splitContainer.Panel1.Controls.Add(panelLeft);
        splitContainer.Panel2.Controls.Add(panelRight);
        Controls.Add(splitContainer);
    }

    private Label CreateHeaderLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Top,
            Height = 45,
            Font = _fontUIBold,
            ForeColor = _textColorDark,
            BackColor = _cardBackColor,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0)
        };
    }

    private Panel CreateCardPanel()
    {
        var panel = new Panel { BackColor = _cardBackColor, Padding = new Padding(1) };
        panel.Paint += (s, e) =>
        {
            using (var pen = new Pen(_borderColor, 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            }
        };
        return panel;
    }

    private void BtnCalculate_Click(object? sender, EventArgs e)
    {
        try
        {
            Cursor = Cursors.WaitCursor;

            string code = txtCode.Text;
            var input = new AntlrInputStream(code);
            var lexer = new Swift5Lexer(input);
            var tokens = new CommonTokenStream(lexer);
            var metrics = new HalsteadMetrics();

            metrics.Calculate(tokens);

            var ops = metrics.Operators.OrderByDescending(x => x.Value).ToList();
            var oprs = metrics.Operands.OrderByDescending(x => x.Value).ToList();

            dgvMainMetrics.Rows.Clear();

            int maxRows = Math.Max(ops.Count, oprs.Count);

            for (int k = 0; k < maxRows; k++)
            {
                string j = k < ops.Count ? $"{k + 1}." : "";
                string opName = k < ops.Count ? ops[k].Key : "";
                string f1j = k < ops.Count ? ops[k].Value.ToString() : "";

                string i = k < oprs.Count ? $"{k + 1}." : "";
                string oprName = k < oprs.Count ? oprs[k].Key : "";
                string f2i = k < oprs.Count ? oprs[k].Value.ToString() : "";

                dgvMainMetrics.Rows.Add(j, opName, f1j, i, oprName, f2i);
            }

            int n1 = metrics.GetUniqueOperators();
            int N1 = metrics.GetTotalOperators();
            int n2 = metrics.GetUniqueOperands();
            int N2 = metrics.GetTotalOperands();

            int vocabulary = metrics.GetVocabulary();
            int length = metrics.GetLength();
            double volume = metrics.GetVolume();
           // bool errors = metrics.GetError();

            int sumRowIndex = dgvMainMetrics.Rows.Add($"η1 = {n1}", "", $"N1 = {N1}", $"η2 = {n2}", "", $"N2 = {N2}");
            dgvMainMetrics.Rows[sumRowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMainMetrics.Rows[sumRowIndex].DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            lblFormulas.Text = 
                $"Словарь программы η = {n1} + {n2} = {vocabulary}.\n" +
                $"Длина программы N = {N1} + {N2} = {length}.\n" +
                $"Объем программы V = {length} * log₂ {vocabulary} = {Math.Round(volume)}.\n";
           // if (errors) lblFormulas.Text += $"Check your code for errors!!!!";
            
          //  metrics.ResetError();
            dgvMainMetrics.ClearSelection();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка анализа", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }
}