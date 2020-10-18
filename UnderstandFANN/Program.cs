using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FANNCSharp;
using FANNCSharp.Double;
using System.Text.RegularExpressions;
using System.Net;

namespace UnderstandFANN
{
     class Program
     {
         static void Main(string[] args)
         {
            // Training data - input
            double[][] inputs =
             {
                 GetFrequencies(true,"appEnglish1"),
                 GetFrequencies(true,"appEnglish2"),
                 GetFrequencies(true,"appEnglish3"),
                 GetFrequencies(true,"appFrancais1"),
                 GetFrequencies(true,"appFrancais2"),
                 GetFrequencies(true,"appFrancais3")
             };
            // Training data - output
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
             layers.Add(34);
             layers.Add(4);
             layers.Add(2);

             NeuralNet network = new NeuralNet(FANNCSharp.NetworkType.LAYER, layers);

            TrainingData data = new TrainingData();
            data.SetTrainData(inputs, outputs);

            network.TrainOnData(data, 3000, 100, 0.001f);

            Console.WriteLine("Final Error :" + network.MSE);

            string again = "Y";
            do { 
            Console.Write("Saisir un text : ");
            string inputtext = Console.ReadLine();
            //
            double[] test = GetFrequencies(false, inputtext);
            double[] result = network.Run(test);

            Console.WriteLine("Anglais : {0}", result[0]);
            Console.WriteLine("Francais : {0}", result[1]);

            double maxValue = result.Max();
            int maxIndex = result.ToList().IndexOf(maxValue);
           // Console.WriteLine("Index {0}", maxIndex);

            switch (maxIndex)
            {
                case 0:
                    Console.WriteLine("Le texte serait en ANGLAIS avec un taux de prédiction de {0} {1}", 100* result[maxIndex], "%");
                    break;
                case 1:
                    Console.WriteLine("Le texte serait en FRANCAIS avec un taux de prédiction de {0} {1} ", 100 * result[maxIndex], "%");
                    break;
                default:
                    Console.WriteLine("Langue non détectée");
                    break;
            }

                Console.Write("Voudriez-vous effectuer un autre test ? (Y/N) : ");
                again = Console.ReadLine();

            } while(again=="Y");
        }

         static double[] GetFrequencies(bool isFile, string txtFilename)
         {
            string text;
            if (isFile)
            {
                 text = System.IO.File.ReadAllText(@"..\texts\" + txtFilename + ".txt").ToString();
            }
            else
            {
                 text = txtFilename;
            }

            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZéàïèùœôê".ToLower().ToCharArray();
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
             double[] freq = new double[34];
              
             for (int i = 0; i < freq.Length; i++)
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
