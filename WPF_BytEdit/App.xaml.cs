using System;
using System.Windows;
using System.Text;
using System.Linq;
using System.Windows.Media;

using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.IO;

namespace WPF_BytEdit
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Variables globales en rapport avec la base de données
        /// </summary>

        //Configuration de la connexion à la base de données
        public static MySqlConnection connection;
        public static string connectionString = "SERVER=127.0.0.1; DATABASE=bytedit; UID=root; PASSWORD=";

        //Variable pour stocker l'id de l'utilisateur
        public static string id_;
        
        /// <summary>
        /// Variables globales pour la génération de chaîne aléatoire
        /// </summary>

        public static Random random = new Random();

        public static string Rdm_Str(int size)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZazertyuiopmlkjhgfdsqwxcvbn0123456789";
            return new string(Enumerable.Repeat(chars, size)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Variables globales pour lire et écrire dans les fichiers de configuration
        /// Les différents fichiers :
        /// - db : pour sauvegarder des données de la base de données
        /// - config : pour toutes stocker toutes les configurations de l'utilisateur
        /// </summary>

        public static string ReadData(string name, string data)
        {
            try
            {
                string line;
                StreamReader file = new StreamReader(name);
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(data))
                    {
                        return line.Substring(line.IndexOf('=') + 1);
                    }
                }
                file.Close();
                return null;
            }
            catch
            {

                return null;
            }
        }

        public static void WriteData(string name, string data, string new_data)
        {
            string line, new_line = null;
            bool here = false;
            try
            {
                StreamReader file_r = new StreamReader(name);
                while ((line = file_r.ReadLine()) != null)
                {
                    if (!line.Contains(data)) new_line += line + "\n";
                    else
                    {
                        new_line += data + "=" + new_data + "\n";
                        here = true;
                    }
                }
                if (!here) new_line += data + "=" + new_data + "\n";
                file_r.Close();
            }
            catch
            {
                new_line += data + "=" + new_data + "\n";
            }
            StreamWriter file_w = new StreamWriter(name, false);
            file_w.WriteLine(new_line);
            file_w.Close();
        }

        /// <summary>
        /// Variables globales pour crypter les données avec le protocole MD5
        /// </summary>

        public static string GetMD5HashData(string data) //Cryptage mdp
        {
            //create new instance of md5
            MD5 md5 = MD5.Create();

            //convert the input text to array of bytes
            byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }

        public static bool ValidateMD5HashData(string inputData, string storedHashData)
        {
            //hash input text and save it string variable
            string getHashInputData = GetMD5HashData(inputData);

            if (string.Compare(getHashInputData, storedHashData) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Variables globales correspondant aux couleurs utilisées dans l'interface
        /// Liste des couleurs :
        /// black (gris très foncé) : pour l'arrière plan de l'éditeur et le texte du dashboard
        /// white : pour l'arrière plan du dashboard et le texte de l'éditeur
        /// or : pour les infos importantes
        /// Pour plus de détails sur les couleurs à utiliser se rapporter aux modèles présents sur Drive
        /// </summary>
         
        public static SolidColorBrush black = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF1A1A1A"));
        public static SolidColorBrush white = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFEFEFEF"));
        public static SolidColorBrush or = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFCC00"));
    }
}
