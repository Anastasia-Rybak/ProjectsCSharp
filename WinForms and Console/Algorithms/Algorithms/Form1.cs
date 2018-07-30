﻿using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace Algorithms
{
    public partial class Form1 : Form
    {
        #region Vars

        private int oldValue;
        private int oldValueCount;
        private Graph graph;
        private DataGridCell cell;
        private bool semaphore;
        private bool semaphore2;
        private int[] path;
        private delegate T[] Sort<T>(T[] massiv, bool revers) where T : IComparable<T>;
        #endregion

        public Form1()
        {
            InitializeComponent();
            oldValueCount = 3;
            dataGridView1.ColumnCount = 2;
            dataGridView1.RowCount = 2;
            dataGridView1.Rows[1].ReadOnly = true;
            dataGridView2.ColumnCount = oldValueCount + 1;
            dataGridView2.RowCount = oldValueCount + 1;
            dataGridView2.Rows[0].ReadOnly = true;
            dataGridView2.Columns[0].ReadOnly = true;
            graph = new Graph("graph");
            AddCellsAndNodes(0, dataGridView2.ColumnCount);
            cell = new DataGridCell(0, 0);
            semaphore = false;
            semaphore2 = false;
        }

        /// <summary>
        /// Алгоритм Флойда
        /// </summary>
        /// <param name="graph">Массив расстояний между узлами</param>
        /// <param name="left">Начальный узел</param>
        /// <param name="right">Конечный узел</param>
        /// <param name="distance">Минимальное расстояние от начального до конечного узла</param>
        /// <returns></returns>
        private int[,] AlgorithmFloyd(int[,] graph, int left, int right, out int distance)
        {
            int n = graph.GetLength(0);
            int[,] path = new int[n, n];
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        int temp = graph[i, k] + graph[k, j];
                        if (graph[i, k] != 0 && graph[k, j] != 0 && temp < graph[i, j])
                        {
                            graph[i, j] = temp;
                            path[i, j] = k + 1;

                        }
                    }
                }
            }
            distance = graph[left, right];
            return path;
        }

        private int[,] AlgorithmFloyd(int[,] graph)
        {
            int n = graph.GetLength(0);
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        int temp = graph[i, k] + graph[k, j];
                        if (graph[i, k] != 0 && graph[k, j] != 0 && temp < graph[i, j])
                        {
                            graph[i, j] = temp;

                        }
                    }
                }
            }
            return graph;
        }


        private int[] GetPathFloyd(int[,] pathFloyd, int left, int right)
        {
            List<int> list = new List<int>() { left };
            list = GetPathFloyd(pathFloyd, left - 1, right - 1, list);
            list.Add(right);
            list.Reverse();
            return list.ToArray();
        }

        private List<int> GetPathFloyd(int[,] pathFloyd, int left, int right, List<int> list)
        {
            int k = pathFloyd[left, right];
            if (k == 0)
                return list;
            list = GetPathFloyd(pathFloyd, left, k - 1, list);
            list.Add(k);
            return GetPathFloyd(pathFloyd, k - 1, right, list);
        }

        /// <summary>
        /// Алгоритм Дейкстры
        /// </summary>
        /// <param name="start">Начальный узел</param>
        /// <param name="graph">Массив расстояний между узлами</param>
        /// <returns>Массив минимальных расстояний</returns>
        int[] AlgorithmDijkstra(int start, int[,] graph)
        {
            bool[] visited = new bool[graph.GetLength(1)];
            int[] distance = new int[graph.GetLength(1)];
            for (int i = 0; i < distance.Length; i++)
            {
                distance[i] = int.MaxValue;
                visited[i] = false;
            }
            distance[start] = 0;
            int minindex, min, temp;
            // Шаг алгоритма
            do
            {
                minindex = graph.GetLength(0);
                min = int.MaxValue;
                for (int i = 0; i < distance.Length; i++)
                { // Если вершину ещё не обошли и вес меньше min
                    if (visited[i] == false && distance[i] < min)
                    { // Переприсваиваем значения
                        min = distance[i];
                        minindex = i;
                    }
                }
                // Добавляем найденный минимальный вес
                // к текущему весу вершины
                // и сравниваем с текущим минимальным весом вершины
                if (minindex != graph.GetLength(0))
                {
                    for (int i = 0; i < distance.Length; i++)
                    {
                        if (graph[minindex, i] > 0)
                        {
                            temp = min + graph[minindex, i];
                            if (temp < distance[i])
                            {
                                distance[i] = temp;
                            }
                        }
                    }
                    visited[minindex] = true;
                }
            } while (minindex < graph.GetLength(0));
            return distance;
        }

        private int[] GetPathDijkstra(int[] distance, int[,] graph, int right)
        {
            int[] path = new int[distance.Length];
            path[0] = right + 1;
            int k = 1; // индекс предыдущей вершины
            int weight = distance[right]; // вес конечной вершины
            while (right > 0) // пока не дошли до начальной вершины
            {
                for (int i = 0; i < path.Length; i++)
                {
                    // просматриваем все вершины
                    if (graph[right, i] != 0)   // если связь есть
                    {
                        int temp = weight - graph[right, i]; // определяем вес пути из предыдущей вершины
                        if (temp == distance[i]) // если вес совпал с рассчитанным
                        {                 // значит из этой вершины и был переход
                            weight = temp; // сохраняем новый вес
                            right = i;       // сохраняем предыдущую вершину
                            path[k] = i + 1; // и записываем ее в массив
                            k++;
                        }
                    }
                }
            }
            return path;
        }

        /// <summary>
        /// Алгоритм Бойера-Мура
        /// </summary>
        /// <param name="text"> Исходный текст</param>
        /// <param name="substring">Образ</param>
        /// <param name="index">Индекс первого вхождения образа в исходном тексте</param>
        /// <returns>Значение, показывающее, содержится ли образ в тексте</returns>
        private bool AlgBM(string text, string substring, out int index)
        {
            int n = text.Length;
            int m = substring.Length;
            int ind;
            int[] table = new int[9999];
            for (int i = 0; i < table.Length; i++)
            {
                table[i] = m;
            }
            for (int i = m - 1; i >= 0; i--)
            {
                if (table[(short)(substring[i])] == m)
                {
                    table[(short)(substring[i])] = m - i - 1;
                }
            }
            ind = m - 1;
            while (ind < n)
            {
                if (substring[m - 1] != text[ind])
                {
                    ind += table[(short)(text[ind])];
                }
                else
                {
                    for (int i = m - 2; i >= 0; i--)
                    {
                        if (substring[i] != text[ind - m + i + 1])
                        {
                            ind += table[(short)(text[ind - m + i + 1])] - 1;
                            break;
                        }
                        else if (i == 0)
                        {
                            index = ind - m + 1;
                            return true;
                        }
                    }
                }
            }
            index = 0;
            return false;
        }

        /// <summary>
        /// Алгоритм Кнута-Морриса-Пратта
        /// </summary>
        /// <param name="text"> Исходный текст</param>
        /// <param name="substring">Образ</param>
        /// <param name="index">Индекс первого вхождения образа в исходном тексте</param>
        /// <returns>Значение, показывающее, содержится ли образ в тексте</returns>
        private bool AlgKMP(string text, string substring, out int index)
        {
            int[] pi = new int[substring.Length];
            int j = 0, i = 1;
            pi[0] = 0;
            while (i != substring.Length)
            {
                if (!substring[i].Equals(substring[j]))
                {
                    if (j == 0)
                    {
                        pi[i] = 0;
                        i++;
                    }
                    else
                    {
                        j = pi[j - 1];
                    }
                }
                else
                {
                    pi[i] = j + 1;
                    i++;
                    j++;
                }
            }
            i = 0;
            j = 0;
            while (i != text.Length)
            {
                if (text[i].Equals(substring[j]))
                {
                    i++;
                    j++;
                    if (j == substring.Length)
                    {
                        index = i - substring.Length;
                        return true;
                    }
                }
                else if (j != 0)
                {
                    j = pi[j - 1];
                }
                else
                {
                    i++;
                    if (i == text.Length)
                    {
                        index = 0;
                        return false;
                    }
                }
            }
            index = 0;
            return false;
        }

        /// <summary>
        /// Сортировка вставками
        /// </summary>
        /// <param name="massiv">Исходный массив элементов</param>
        /// <param name="revers">Значение true, если требуется отсортировать массив по убыванию, иначе - false</param>
        /// <returns>Отсортированный массив элементов</returns>
        private T[] InsertionSort<T>(T[] massiv, bool revers) where T : IComparable<T>
        {
            for (int i = 1; i < massiv.Length; i++)
            {
                T x = massiv[i];
                int j = i;
                while (j > 0 && ((revers && massiv[j - 1].CompareTo(x) < 0) || (!revers && massiv[j - 1].CompareTo(x) > 0)))
                {
                    massiv[j] = massiv[j - 1];
                    j--;
                }
                massiv[j] = x;
            }
            return massiv;
        }

        /// <summary>
        /// Сортировка пузырьком
        /// </summary>
        /// <param name="massiv">Исходный массив элементов</param>
        /// <param name="revers">Значение true, если требуется отсортировать массив по убыванию, иначе - false</param>
        /// <returns>Отсортированный массив элементов</returns>
        private T[] BubbleSort<T>(T[] massiv, bool revers) where T : IComparable<T>
        {
            for (int i = 0; i < massiv.Length; i++)
            {
                bool flag = false;
                for (int j = 0; j < massiv.Length - i - 1; j++)
                {
                    if ((revers && massiv[j].CompareTo(massiv[j + 1]) < 0) ||
                        (!revers && massiv[j].CompareTo(massiv[j + 1]) > 0))
                    {
                        massiv = Swap(massiv, j, j + 1);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    break;
                }
            }
            return massiv;
        }

        /// <summary>
        /// Сортировка выбором
        /// </summary>
        /// <param name="massiv">Исходный массив элементов</param>
        /// <param name="revers">Значение true, если требуется отсортировать массив по убыванию, иначе - false</param>
        /// <returns>Отсортированный массив элементов</returns>
        private T[] SelectionSort<T>(T[] massiv, bool revers) where T : IComparable<T>
        {
            for (int i = 0; i < massiv.Length - 1; i++)
            {
                int index = i;
                for (int j = i + 1; j < massiv.Length; j++)
                {
                    if ((revers && massiv[j].CompareTo(massiv[index]) > 0) ||
                        (!revers && massiv[j].CompareTo(massiv[index]) < 0))
                    {
                        index = j;
                    }
                }
                if (index != i)
                {
                    massiv = Swap(massiv, i, index);
                }
            }
            return massiv;
        }

        int Sift<T>(T[] massiv, int i, int N, bool revers) where T : IComparable<T>
        {
            int imax;
            if (2 * i + 2 < N)
            {
                if ((!revers && massiv[2 * i + 1].CompareTo(massiv[2 * i + 2]) < 0) ||
                    revers && massiv[2 * i + 1].CompareTo(massiv[2 * i + 2]) > 0)
                    imax = 2 * i + 2;
                else
                    imax = 2 * i + 1;
            }
            else
                imax = 2 * i + 1;
            if (imax >= N)
                return i;
            if ((!revers && massiv[i].CompareTo(massiv[imax]) < 0) || (revers && massiv[i].CompareTo(massiv[imax]) > 0))
            {
                massiv = Swap(massiv, i, imax);
                if (imax < N / 2)
                    i = imax;
            }
            return i;
        }

        /// <summary>
        /// Пирамидальная сортировка
        /// </summary>
        /// <param name="massiv">Исходный массив элементов</param>
        /// <param name="revers">Значение true, если требуется отсортировать массив по убыванию, иначе - false</param>
        /// <returns>Отсортированный массив элементов</returns>
        public T[] HeapSort<T>(T[] massiv, bool revers) where T : IComparable<T>
        {
            for (int i = massiv.Length / 2 - 1; i >= 0; --i)
            {
                int prev_i = i;
                i = Sift(massiv, i, massiv.Length, revers);
                if (prev_i != i) ++i;
            }
            for (int k = massiv.Length - 1; k > 0; --k)
            {
                massiv = Swap(massiv, 0, k);
                int i = 0, prev_i = -1;
                while (i != prev_i)
                {
                    prev_i = i;
                    i = Sift(massiv, i, k, revers);
                }
            }
            return massiv;
        }

        /// <summary>
        /// Сортировка слиянием
        /// </summary>
        /// <param name="massiv">Исходный массив элементов</param>
        /// <param name="revers">Значение true, если требуется отсортировать массив по убыванию, иначе - false</param>
        /// <returns>Отсортированный массив элементов</returns>
        private T[] MergeSort<T>(T[] massiv, bool revers) where T : IComparable<T>
        {
            if (massiv.Length == 1)
                return massiv;
            return Merge(MergeSort(massiv.Take(massiv.Length / 2).ToArray(), revers), MergeSort(massiv.Skip(massiv.Length / 2).ToArray(), revers), revers);
        }

        private T[] Merge<T>(T[] massiv1, T[] massiv2, bool revers) where T : IComparable<T>
        {
            int a = 0, b = 0;
            T[] massiv = new T[massiv1.Length + massiv2.Length];
            for (int i = 0; i < massiv1.Length + massiv2.Length; i++)
            {
                if (b < massiv2.Length && a < massiv1.Length)
                {
                    if ((!revers && massiv1[a].CompareTo(massiv2[b]) > 0) ||
                        (revers && massiv1[a].CompareTo(massiv2[b]) < 0))
                    {
                        massiv[i] = massiv2[b++];
                    }
                    else massiv[i] = massiv1[a++];
                }
                else
                {
                    if (b < massiv2.Length)
                    {
                        massiv[i] = massiv2[b++];
                    }
                    else
                    {
                        massiv[i] = massiv1[a++];
                    }
                }
            }
            return massiv;
        }

        private int Partition<T>(T[] massiv, int start, int end, bool revers) where T : IComparable<T>
        {
            int marker = start;
            for (int i = start; i <= end; i++)
            {
                if ((!revers && massiv[i].CompareTo(massiv[end]) <= 0) ||
                    (revers && massiv[i].CompareTo(massiv[end]) >= 0))
                {
                    massiv = Swap(massiv, marker, i);
                    marker += 1;
                }
            }
            return marker - 1;
        }

        /// <summary>
        /// Быстрая сортировка
        /// </summary>
        /// <param name="massiv">Исходный массив элементов</param>
        /// <param name="revers">Значение true, если требуется отсортировать массив по убыванию, иначе - false</param>
        /// <returns>Отсортированный массив элементов</returns>
        private T[] QuickSort<T>(T[] massiv, bool revers) where T : IComparable<T>
        {
            int pivot = Partition(massiv, 0, massiv.Length - 1, revers);
            massiv = QuickSort(massiv, 0, pivot - 1, revers);
            massiv = QuickSort(massiv, pivot + 1, massiv.Length - 1, revers);
            return massiv;
        }

        private T[] QuickSort<T>(T[] massiv, int start, int end, bool revers) where T : IComparable<T>
        {
            if (start >= end)
            {
                return massiv;
            }
            int pivot = Partition(massiv, start, end, revers);
            massiv = QuickSort(massiv, start, pivot - 1, revers);
            massiv = QuickSort(massiv, pivot + 1, end, revers);
            return massiv;
        }

        /// <summary>
        /// Сортировка подсчетом
        /// </summary>
        /// <param name="massiv">Исходный массив элементов</param>
        /// <param name="revers">Значение true, если требуется отсортировать массив по убыванию, иначе - false</param>
        /// <returns>Отсортированный массив элементов</returns>
        private T[] CountingSort<T>(T[] massiv, bool revers) where T : IComparable<T>
        {
            T[] massiv_out = new T[massiv.Length];
            int[] counts = new int[massiv.Length];
            for (int i = 1; i < massiv.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if ((!revers && massiv[i].CompareTo(massiv[j]) < 0) ||
                        (revers && massiv[i].CompareTo(massiv[j]) > 0))
                    {
                        counts[j]++;
                    }
                    else counts[i]++;
                }
            }
            for (int i = 0; i < massiv.Length; i++)
            {
                massiv_out[counts[i]] = massiv[i];
            }
            return massiv_out;
        }

        /// <summary>
        /// "Шейкерная" сортировка
        /// </summary>
        /// <param name="massiv">Исходный массив элементов</param>
        /// <param name="revers">Значение true, если требуется отсортировать массив по убыванию, иначе - false</param>
        /// <returns>Отсортированный массив элементов</returns>
        private T[] ShakerSort<T>(T[] massiv, bool revers) where T : IComparable<T>
        {
            int left = 0,
                right = massiv.Length - 1,
                count = 0;

            while (left <= right)
            {
                for (int i = left; i < right; i++)
                {
                    count++;
                    if ((!revers && massiv[i].CompareTo(massiv[i + 1]) > 0) ||
                        (revers && massiv[i].CompareTo(massiv[i + 1]) < 0))
                        massiv = Swap(massiv, i, i + 1);
                }
                right--;
                for (int i = right; i > left; i--)
                {
                    count++;
                    if ((!revers && massiv[i - 1].CompareTo(massiv[i]) > 0) ||
                        (revers && massiv[i - 1].CompareTo(massiv[i]) < 0))
                        massiv = Swap(massiv, i - 1, i);
                }
                left++;
            }
            return massiv;
        }

        private T[] Swap<T>(T[] massiv, int i, int j)
        {
            T temp = massiv[i];
            massiv[i] = massiv[j];
            massiv[j] = temp;
            return massiv;
        }

        private void AddCellsAndNodes(int left, int right)
        {
            for (int i = left + 1; i < right; i++)
            {
                DataGridViewButtonCell buttonCell = new DataGridViewButtonCell();
                buttonCell.Value = i;
                dataGridView2[0, i] = buttonCell;
                buttonCell = new DataGridViewButtonCell();
                buttonCell.Value = i;
                dataGridView2[i, 0] = buttonCell;
                dataGridView2[i, i].Value = 0;
                dataGridView2[i, i].Style.BackColor = System.Drawing.Color.Yellow;
                dataGridView2[i, i].ReadOnly = true;
                graph.AddNode(i.ToString());
            }
        }

        private void RemoveNodes(int left, int right)
        {
            for (int i = left; i < right + 1; i++)
            {
                try
                {
                    graph.RemoveNode(graph.FindNode(i.ToString()));
                }
                catch
                {

                }
            }
        }

        private void ShowGraph(Graph graph)
        {
            GraphForm graphForm = new GraphForm(graph);
            graphForm.ShowDialog();
        }

        private void UpdateLabel(int left, int right, double distance)
        {
            label8.Text = string.Format("Минимальное расстояние из узла {0} в узел {1} равно {2}.", left, right, distance);
        }

        private void UpdateLabel(double index)
        {
            if (index != 0)
            {
                label8.Text = string.Format("Центр графа - вершина под номером {0}.", index);
            }
            else label8.Text = "Невозможно определить центр графа.";
        }
        private void UpdateLabel(string text)
        {
            label8.Text = text;
        }

        private void ButtonVisible(bool value)
        {
            button5.Visible = value;
        }

        private void SetEdgeColor(int[] path, Color color)
        {
            for (int i = path.Length - 1; i > 0; i--)
            {
                if (path[i] != 0)
                {
                    try
                    {
                        graph.FindNode(path[i].ToString()).Attr.Color = color;
                        graph.FindNode(path[i - 1].ToString()).Attr.Color = color;
                        graph.Edges.First(item => (item.Source.Equals(path[i].ToString()) && item.Target.Equals(path[i - 1].ToString()))).Attr.Color = color;
                    }
                    catch
                    {
                        graph.Edges.First(item => (item.Source.Equals(path[i - 1].ToString()) && item.Target.Equals(path[i].ToString()))).Attr.Color = color;
                    }
                }
            }
        }

        private int[,] GetMassiv(int rowCount, int columnCount)
        {
            int[,] massiv = new int[rowCount, columnCount];
            for (int i = 0; i < massiv.GetLength(0); i++)
            {
                for (int j = 0; j < massiv.GetLength(1); j++)
                {
                    try
                    {
                        massiv[i, j] = Convert.ToInt32(dataGridView2[j + 1, i + 1].Value);

                    }
                    catch
                    {
                        massiv[i, j] = 0;
                    }
                }
            }
            return massiv;
        }

        private int MaxInColumn(int index, int[,] massiv)
        {
            int max = massiv[0, index];
            for (int i = 1; i < massiv.GetLength(0); i++)
            {
                if (max < massiv[i, index] && massiv[i, index] != 0)
                {
                    max = massiv[i, index];
                }
            }
            return max;
        }

        private int IndexMinInMassiv(int[] massiv)
        {
            int min = massiv[0];
            int index = 0;
            for (int i = 1; i < massiv.Length; i++)
            {
                if (min > massiv[i] && massiv[i] != 0)
                {
                    min = massiv[i];
                    index = i + 1;
                }
            }
            return index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int k = -1;
            if (richTextBox1.Text == string.Empty)
            {
                label3.Text = "Ошибка: Исходный текст не задан.";
            }
            else if (textBox2.Text == string.Empty)
            {
                label3.Text = "Ошибка: Образ не задан.";
            }
            else if (richTextBox1.Text.Length < textBox2.Text.Length)
            {
                label3.Text = "Ошибка: Длина образа больше длины исходного текста.";
            }
            else if ((radioButton1.Checked && AlgKMP(richTextBox1.Text, textBox2.Text, out k)) ||
                     (radioButton2.Checked && AlgBM(richTextBox1.Text, textBox2.Text, out k)))
            {
                label3.Text = "Вывод: Образ найден. Индекс первого вхождения: " + k.ToString() + ".";
            }
            else
            {
                label3.Text = "Вывод: Образ не найден.";
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.ColumnCount = Convert.ToInt32(numericUpDown1.Value);
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 50 && e.KeyChar > 57) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, System.Windows.Forms.DataGridViewCellCancelEventArgs e)
        {
            try
            {
                DataGridView dataGridView = sender as DataGridView;
                oldValue = Convert.ToInt32(dataGridView[e.ColumnIndex, e.RowIndex].Value);
            }
            catch
            {
                oldValue = 0;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int temp = Convert.ToInt32(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
            }
            catch
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = oldValue;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[] massiv = new int[dataGridView1.ColumnCount];
            Sort<int> sort;
            for (int i = 0; i < massiv.Length; i++)
            {
                massiv[i] = Convert.ToInt32(dataGridView1[i, 0].Value);
            }
            if (radioButton3.Checked)
            {
                sort = SelectionSort;
            }
            else if (radioButton4.Checked)
            {
                sort = InsertionSort;
            }
            else if (radioButton5.Checked)
            {
                sort = BubbleSort;
            }
            else if (radioButton9.Checked)
            {
                sort = HeapSort;
            }
            else if (radioButton10.Checked)
            {
                sort = QuickSort;
            }
            else if (radioButton11.Checked)
            {
                sort = CountingSort;
            }
            else if (radioButton12.Checked)
            {
                sort = ShakerSort;
            }
            else
            {
                sort = MergeSort;
            }
            massiv = sort.Invoke(massiv, checkBox1.Checked);
            for (int i = 0; i < massiv.Length; i++)
            {
                dataGridView1[i, 1].Value = massiv[i];
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown3.Maximum = numericUpDown2.Value;
            numericUpDown4.Maximum = numericUpDown2.Value;
            dataGridView2.ColumnCount = Convert.ToInt32(numericUpDown2.Value) + 1;
            dataGridView2.RowCount = dataGridView2.ColumnCount;
            if (dataGridView2.ColumnCount - 1 > oldValueCount)
            {
                AddCellsAndNodes(oldValueCount, dataGridView2.ColumnCount);
            }
            else if (dataGridView2.ColumnCount - 1 < oldValueCount)
            {
                RemoveNodes(dataGridView2.ColumnCount, oldValueCount);
            }
            oldValueCount = dataGridView2.ColumnCount - 1;
        }

        private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 51 && e.KeyChar > 57) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (semaphore2)
            {
                semaphore2 = !semaphore2;
                return;
            }
            try
            {
                if (e.RowIndex != e.ColumnIndex && !semaphore2)
                {
                    int temp = Convert.ToInt32(dataGridView2[e.ColumnIndex, e.RowIndex].Value);
                    if (temp < 0)
                        throw new Exception();
                    semaphore2 = !semaphore2;
                    bool isFound = false;
                    dataGridView2[e.RowIndex, e.ColumnIndex].Value = temp;
                    if (dataGridView2[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        semaphore2 = !semaphore2;
                        dataGridView2[e.ColumnIndex, e.RowIndex].Value = temp;
                    }
                    if (temp != 0)
                    {
                        try
                        {
                            graph.Edges.First(item => (item.Source.Equals(e.ColumnIndex.ToString()) && item.Target.Equals(e.RowIndex.ToString()))
                                                     || (item.Source.Equals(e.RowIndex.ToString()) && item.Target.Equals(e.ColumnIndex.ToString()))).LabelText = temp.ToString();
                            isFound = true;
                        }
                        catch
                        {

                        }
                    }
                    if (!isFound)
                    {
                        if (temp != 0)
                        {
                            graph.AddEdge(e.RowIndex.ToString(), temp.ToString(), e.ColumnIndex.ToString()).Attr.ArrowheadAtTarget = ArrowStyle.None;
                        }
                        else
                        {
                            try
                            {
                                graph.RemoveEdge(graph.Edges.First(item => (item.Source.Equals(e.ColumnIndex.ToString()) && item.Target.Equals(e.RowIndex.ToString()))
                                                         || (item.Source.Equals(e.RowIndex.ToString()) && item.Target.Equals(e.ColumnIndex.ToString()))));
                            }
                            catch
                            {

                            }
                        }
                    }
                    ButtonVisible(false);
                    UpdateLabel(string.Empty);
                }
            }
            catch
            {
                semaphore2 = !semaphore2;
                dataGridView2[e.ColumnIndex, e.RowIndex].Value = oldValue;
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!semaphore && cell.ColumnNumber != 0 && cell.RowNumber != 0)
            {
                dataGridView2[cell.ColumnNumber, 0].Style.BackColor = System.Drawing.Color.White;
                dataGridView2[0, cell.RowNumber].Style.BackColor = System.Drawing.Color.White;
                if (cell.ColumnNumber == cell.RowNumber)
                {
                    dataGridView2[cell.ColumnNumber, cell.RowNumber].Style.BackColor = System.Drawing.Color.Yellow;
                }
                else
                {
                    dataGridView2[cell.ColumnNumber, cell.RowNumber].Style.BackColor = System.Drawing.Color.White;
                }
                cell.ColumnNumber = 0;
                cell.RowNumber = 0;
            }
            if (e.RowIndex != e.ColumnIndex)
            {
                if (e.RowIndex == 0)
                {
                    if (semaphore)
                    {
                        if (cell.ColumnNumber != 0)
                        {
                            dataGridView2[cell.ColumnNumber, 0].Style.BackColor = System.Drawing.Color.White;
                            dataGridView2[e.ColumnIndex, 0].Style.BackColor = System.Drawing.Color.Red;
                            cell.ColumnNumber = e.ColumnIndex;
                        }
                        else
                        {
                            cell.ColumnNumber = e.ColumnIndex;
                            dataGridView2[cell.ColumnNumber, cell.RowNumber].Style.BackColor = System.Drawing.Color.Red;
                            semaphore = !semaphore;
                        }
                    }
                    else
                    {
                        dataGridView2[e.ColumnIndex, 0].Style.BackColor = System.Drawing.Color.Red;
                        cell.ColumnNumber = e.ColumnIndex;
                        semaphore = !semaphore;
                    }
                }
                else if (e.ColumnIndex == 0)
                {
                    if (semaphore)
                    {
                        if (cell.RowNumber != 0)
                        {
                            dataGridView2[0, cell.RowNumber].Style.BackColor = System.Drawing.Color.White;
                            dataGridView2[0, e.RowIndex].Style.BackColor = System.Drawing.Color.Red;
                            cell.RowNumber = e.RowIndex;
                        }
                        else
                        {
                            cell.RowNumber = e.RowIndex;
                            dataGridView2[cell.ColumnNumber, cell.RowNumber].Style.BackColor = System.Drawing.Color.Red;
                            semaphore = !semaphore;
                        }
                    }
                    else
                    {
                        dataGridView2[0, e.RowIndex].Style.BackColor = System.Drawing.Color.Red;
                        cell.RowNumber = e.RowIndex;
                        semaphore = !semaphore;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowGraph(graph);
        }

        private void numericUpDown3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 49 && e.KeyChar > 57) && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int left = Convert.ToInt32(numericUpDown3.Value);
            int right = Convert.ToInt32(numericUpDown4.Value);
            if (right != left)
            {
                int[,] massiv = GetMassiv(dataGridView2.RowCount - 1, dataGridView2.ColumnCount - 1);
                if (radioButton7.Checked)
                {
                    int[] distance = AlgorithmDijkstra(left - 1, massiv);
                    if (distance[right - 1] != int.MaxValue)
                    {
                        UpdateLabel(left, right, distance[right - 1]);
                        path = GetPathDijkstra(distance, massiv, right - 1);
                        ButtonVisible(true);
                    }
                    else
                    {
                        UpdateLabel(left, right, double.PositiveInfinity);
                        ButtonVisible(false);
                    }
                }
                else
                {
                    int distance;
                    int[,] pathFloyd = AlgorithmFloyd(massiv, left - 1, right - 1, out distance);
                    if (distance != 0)
                    {
                        UpdateLabel(left, right, distance);
                        path = GetPathFloyd(pathFloyd, left, right);
                        ButtonVisible(true);
                    }
                    else
                    {
                        UpdateLabel(left, right, double.PositiveInfinity);
                        ButtonVisible(false);
                    }
                }

            }
            else
            {
                UpdateLabel("Ошибка: Начальный и конечный узлы равны.");
                ButtonVisible(false);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetEdgeColor(path, Color.Red);
            ShowGraph(graph);
            SetEdgeColor(path, Color.Black);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int[,] graph = AlgorithmFloyd(GetMassiv(dataGridView2.RowCount - 1, dataGridView2.ColumnCount - 1));
            int[] maxDistances = new int[graph.GetLength(1)];
            for (int i = 0; i < maxDistances.Length; i++)
            {
                maxDistances[i] = MaxInColumn(i, graph);
            }
            UpdateLabel(IndexMinInMassiv(maxDistances));
        }
    }
}
