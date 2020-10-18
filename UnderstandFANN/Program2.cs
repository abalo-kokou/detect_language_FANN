using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FANNCSharp;
using FANNCSharp.Double;
using System.Text.RegularExpressions;

namespace UnderstandFANN
{
     class Program2
     {
        private static string inputext = "";
        public static string Inputext
        {
            get { return inputext; }
            set { inputext = value; }

        }
        static void Main(string[] args)
         {
             
             double[][] inputs =
             {
                 GetFrequencies("appEnglish1"),
                 GetFrequencies("appEnglish2"),
                 GetFrequencies("appEnglish3"),
                 GetFrequencies("appFrancais1"),
                 GetFrequencies("appFrancais2"),
                 GetFrequencies("appFrancais3")
             };

             double[][] outputs =
             {
                 new double[]{1,0},
                 new double[]{1,0},
                 new double[]{1,0},
                 new double[]{0,1},
                 new double[]{0,1},
                 new double[]{0,1}
             };

             List<uint> layers = new List<uint>();
             layers.Add(7);
             layers.Add(4);
             layers.Add(2);

             NeuralNet network = new NeuralNet(FANNCSharp.NetworkType.LAYER, layers);

             TrainingData data = new TrainingData();
             data.SetTrainData(inputs, outputs);

             network.TrainOnData(data, 3000, 100, 0.001f);

             Console.WriteLine("Final Error :" + network.MSE);


            Language_Detector f2 = new Language_Detector();
            f2.ShowDialog();
            inputext = f2.richTextBox1.Text;
            
            double[] test = GetFrequencies("textEnglish");
            //double[] test = GetFrequencies("textFrancais");
            
            //double[] test = detectLanguage("Prédiction");
            double[] result = network.Run(test);


             Console.WriteLine("Anglais : {0}", result[0]);
             Console.WriteLine("Francais : {0}", result[1]);
             //Console.WriteLine("Polonais : {0}", result[2]);

             double maxValue = result.Max();
             int maxIndex = result.ToList().IndexOf(maxValue);
             Console.WriteLine("Index {0}", maxIndex);

            switch (maxIndex)
            {
                case 0:
                    Console.WriteLine("Le texte serait en ANGLAIS avec un taux de prédiction de {0} {1}", 100* result[maxIndex], "%");
                    break;
                case 1:
                    Console.WriteLine("Le texte serait en FRANCAIS avec un taux de prédiction de {0} {1} ", 100 * result[maxIndex], "%");
                    break;
                default:
                    Console.WriteLine("Le texte serait en Polonais avec un taux de prédiction de {0} {1} ", 100 * result[maxIndex], "%");
                    break;
            }
        }

         static double[] GetFrequencies(string txtFilename)
         {
             string text = System.IO.File.ReadAllText(@"..\texts\" + txtFilename + ".txt").ToString();
             byte[] tempBytes;
             tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(text);
             string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
             //string[] lettres = new string[7];

             char[] alpha = "éàïèùœô".ToLower().ToCharArray();
            // Console.WriteLine("-------------- {0}", alpha[0]);
            //char[] alpha = "éàïèùœô".ToLower().ToCharArray();
           // Console.WriteLine("-------text------- {0}", text);
            string textisLetter = "";
             for (int i = 0; i < text.Length; i++)
             {
                 if (Char.IsLetter(text[i]))
                 {
                     textisLetter += text[i];
                 }
             }

             Dictionary<char, int> dict = textisLetter.ToLower().GroupBy(c => c)
                              .ToDictionary(gr => gr.Key, gr => gr.Count());

             foreach (var item in dict.Keys)
             {
                 Console.WriteLine(item + " : " + dict[item]);
             }
             int sum = dict.Sum(x => x.Value);
             double[] freq = new double[7];
              
             for (int i = 0; i < freq.Length; i++)
             {
                 int value = 0;
                 if (dict.TryGetValue(alpha[i], out value))
                 {
                     freq[i] = (double)value / (double)sum;
                     freq[i] = Math.Truncate(freq[i] * 1000) / 1000;
                     Console.WriteLine("----value---------- {0}", value);
                 }
                 else
                 {
                     freq[i] = 0.0;
                   // Console.WriteLine("----value---------- {0}", value);
                }
                 Console.WriteLine(freq[i]);
             }

            double res = 0;
             foreach (double d in freq)
             {
                 res += d;
             }
             Console.WriteLine(res);
             return freq;
         }
        static double[] detectLanguage(string txtFilename)
        {
            string text = txtFilename;
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(text);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            string[] lettres = new string[33];

            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZéàïèùœô".ToLower().ToCharArray();
            string textisLetter = "";
            for (int i = 0; i < asciiStr.Length; i++)
            {
                if (Char.IsLetter(asciiStr[i]))
                {
                    textisLetter += asciiStr[i];
                }
            }

            Dictionary<char, int> dict = textisLetter.ToLower().GroupBy(c => c)
                             .ToDictionary(gr => gr.Key, gr => gr.Count());

            foreach (var item in dict.Keys)
            {
                Console.WriteLine(item + " : " + dict[item]);
            }
            int sum = dict.Sum(x => x.Value);
            double[] freq = new double[33];
            for (int i = 0; i < 33; i++)
            {
                int value = 0;
                if (dict.TryGetValue(alpha[i], out value))
                {
                    freq[i] = (double)value / (double)sum;
                    freq[i] = Math.Truncate(freq[i] * 1000) / 1000;
                }
                else
                {
                    freq[i] = 0.0;
                }
                Console.WriteLine(freq[i]);
            }
            double res = 0;
            foreach (double d in freq)
            {
                res += d;
            }
            Console.WriteLine(res);
            return freq;
        }
    }
}
