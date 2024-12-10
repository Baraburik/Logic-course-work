using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace курсач__WF_
{
    public partial class ManualGraphInputForm : Form
    {
        private int graphSize;
        private DataGridView dgv;

        public ManualGraphInputForm(int size)
        {
            InitializeComponent();
            graphSize = size;
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dgv = new DataGridView
            {
                ColumnCount = graphSize + 1, // Первая колонка для номеров вершин
                RowCount = graphSize, // Столько же строк, сколько и вершин
                Dock = DockStyle.Fill
            };

            // Создаем заголовки столбцов
            dgv.Columns[0].Name = "Вершина";
            for (int i = 1; i <= graphSize; i++)
            {
                dgv.Columns[i].Name = i.ToString();
            }

            // Заполняем таблицу нулями (по умолчанию нет соединений)
            for (int i = 0; i < graphSize; i++)
            {
                int ii = i + 1;
                dgv.Rows[i].Cells[0].Value = ii.ToString();
                for (int j = 1; j <= graphSize; j++)
                {
                    dgv.Rows[i].Cells[j].Value = 0; // 0 - значит, нет ребра
                }
            }

            // Добавляем кнопку сохранения
            Button btnSave = new Button
            {
                Text = "Сохранить",
                Dock = DockStyle.Bottom
            };
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(dgv);
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            List<List<int>> graph = new List<List<int>>();

            // Чтение данных из таблицы и формирование списка смежности
            for (int i = 0; i < graphSize; i++)
            {
                var neighbors = new List<int>();
                for (int j = 1; j <= graphSize; j++)
                {
                    if (Convert.ToInt32(dgv.Rows[i].Cells[j].Value) == 1)
                    {
                        neighbors.Add(j - 1); // Сосед по индексу
                    }
                }
                graph.Add(neighbors);
            }

            // Возвращаем граф в основную форму
            this.DialogResult = DialogResult.OK;
            this.Tag = graph;
            this.Close();
        }

        public List<List<int>> GetGraph()
        {
            return (List<List<int>>)this.Tag;
        }

        private void ManualGraphInputForm_Load(object sender, EventArgs e)
        {

        }
    }
}
