﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Algorithms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void button1_Click(object sender, EventArgs e)
        {
            int k = 0;
            if (textBox1.Text == string.Empty)
            {
                label3.Text = "Ошибка: Исходный текст не задан.";
            }
            else if (textBox2.Text == string.Empty)
            {
                label3.Text = "Ошибка: Образ не задан.";
            }
            else if (textBox1.Text.Length < textBox2.Text.Length)
            {
                label3.Text = "Ошибка: Длина образа больше длины исходного текста.";
            }
            else if (AlgKMP(textBox1.Text, textBox2.Text, out k))
            {
                label3.Text = "Вывод: Образ найден. Индекс первого вхождения: " + k.ToString() + ".";
            }
            else
            {
                label3.Text = "Вывод: Образ не найден.";
            }
        }
    }
}
