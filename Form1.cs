using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace курсач__WF_
{
    public partial class Form1 : Form
    {
        private int graphSize = 5; // Размер графа
        private List<List<int>> graph; // Список смежности
        private int[] colors; // Массив цветов для вершин
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            graph = new List<List<int>>();
            colors = new int[graphSize];
            InitializeGraph();
        }

        private void InitializeGraph()
        {
            // Инициализация пустого графа
            graph.Clear();
            for (int i = 0; i < graphSize; i++)
            {
                graph.Add(new List<int>());
            }
        }

        private void DrawGraph()
        {
            // Очистить экран
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);

            // Нарисовать вершины
            int radius = 20;
            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;
            int offsetX = 100, offsetY = 100;

            for (int i = 0; i < graphSize; i++)
            {
                int angle = i * (360 / graphSize);
                int x = centerX + (int)(offsetX * Math.Cos(Math.PI * angle / 180));
                int y = centerY + (int)(offsetY * Math.Sin(Math.PI * angle / 180));
                Brush vertexColor = new SolidBrush(GetColorForVertex(i));

                int ii = i + 1;
                g.FillEllipse(vertexColor, x - radius, y - radius, radius * 2, radius * 2);
                g.DrawString(ii.ToString(), new Font("Arial", 10), Brushes.Black, x - radius / 2, y - radius / 2);
            }

            // Нарисовать ребра
            for (int i = 0; i < graphSize; i++)
            {
                foreach (var neighbor in graph[i])
                {
                    int x1 = centerX + (int)(offsetX * Math.Cos(Math.PI * i * (360 / graphSize) / 180));
                    int y1 = centerY + (int)(offsetY * Math.Sin(Math.PI * i * (360 / graphSize) / 180));

                    int x2 = centerX + (int)(offsetX * Math.Cos(Math.PI * neighbor * (360 / graphSize) / 180));
                    int y2 = centerY + (int)(offsetY * Math.Sin(Math.PI * neighbor * (360 / graphSize) / 180));

                    g.DrawLine(Pens.Black, x1, y1, x2, y2);
                }
            }

            pictureBox1.Invalidate();
        }

        private Color GetColorForVertex(int vertex)
        {
            // Возвращаем цвет для вершины в зависимости от ее раскраски
            switch (colors[vertex])
            {
                case 0: return Color.Red;
                case 1: return Color.Orange;
                case 2: return Color.Yellow;
                case 3: return Color.Green;
                case 4: return Color.Blue;
                case 5: return Color.Purple;
                default: return Color.Gray;
            }
        }

        private void ColorGraph()
        {
            // Применяем жадный алгоритм раскраски
            for (int i = 0; i < graphSize; i++)
            {
                bool[] availableColors = new bool[graphSize];

                // Проверяем соседей
                foreach (var neighbor in graph[i])
                {
                    if (colors[neighbor] != -1)
                    {
                        availableColors[colors[neighbor]] = true;
                    }
                }

                // Находим первый доступный цвет
                int color = -1;
                for (int j = 0; j < availableColors.Length; j++)
                {
                    if (!availableColors[j])
                    {
                        color = j;
                        break;
                    }
                }

                colors[i] = color;
            }

            DrawGraph();
        }

private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CreateGraph_Click(object sender, EventArgs e)
        {
            // Генерация случайного графа
            graphSize = int.Parse(txtGraphSize.Text);
            InitializeGraph();
            for (int i = 0; i < graphSize; i++)
            {
                for (int j = i + 1; j < graphSize; j++)
                {
                    if (random.Next(2) == 1)
                    {
                        graph[i].Add(j);
                        graph[j].Add(i);
                    }
                }
            }
            colors = new int[graphSize]; // Сброс раскраски
            ColorGraph();
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
        }

        private void ManualGraph_Click(object sender, EventArgs e)
        {
            graphSize = int.Parse(txtGraphSize.Text);
            var manualInputForm = new ManualGraphInputForm(graphSize);
            if (manualInputForm.ShowDialog() == DialogResult.OK)
            {
                graph = manualInputForm.GetGraph();
                colors = new int[graphSize]; // Сброс раскраски
                ColorGraph();
            }
        }

        private void SaveImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Сохраняем изображение
                    pictureBox1.Image.Save(saveDialog.FileName);
                    MessageBox.Show("Изображение успешно сохранено!", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении изображения: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    graphSize = int.Parse(lines[0]); // Размер графа
                    InitializeGraph();
                    colors = new int[graphSize]; // Сброс раскраски

                    // Читаем матрицу смежности
                    for (int i = 0; i < graphSize; i++)
                    {
                        string[] row = lines[i + 1].Split(' ');
                        for (int j = 0; j < graphSize; j++)
                        {
                            if (row[j] == "1") // Если связь существует
                            {
                                graph[i].Add(j);
                            }
                        }
                    }
                    ColorGraph();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при чтении файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
