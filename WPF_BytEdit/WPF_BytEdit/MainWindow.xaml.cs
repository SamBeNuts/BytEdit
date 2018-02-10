using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;

namespace WPF_BytEdit
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Configuration de la connexion à la base de données
        private MySqlConnection connection;
        string connectionString = "SERVER=127.0.0.1; DATABASE=bytedit; UID=root; PASSWORD=";
        bool connexion;

        private static Random random = new Random();
        int mode, etape; //mode=signin/signup et etape correspond à l'etape dans le formulaire de signin(max 2)/signup(max 3)
        string pwd; //pour stocker l'identfiant et le mot de passe

        public MainWindow()
        {
            connection = new MySqlConnection(connectionString); //Connexion à la bdd
            connexion = false;

            try
            {
                connection.Open();

                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT remember_token FROM users WHERE name=@name";
                cmd.Parameters.AddWithValue("@name", ReadData("db", "name"));

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["remember_token"].ToString() == ReadData("db", "token"))
                    {
                        connexion = true;
                    }
                }
                rdr.Close();
                connection.Close();
                if (connexion) Connected();
            }
            catch
            {

            }

            InitializeComponent();

            mode = 0; //par défaut signin
            etape = 0; //init
        }

        private void EtapeSuivante() //Change d'etape et actualise le formulaire
        {
            if (mode == 0)
            {
                if (etape == 0)
                {
                    if (textbox.Text.Length < 4)
                    {
                        Erreur(0);
                    }
                    else
                    {
                        etape++;
                        placeholder.Content = "Mot de passe";
                        textbox.Visibility = Visibility.Hidden;
                        passwordbox.Visibility = Visibility.Visible;
                        next_icon_png.RenderTransform = new RotateTransform(90);
                        passwordbox.Focus();
                        placeholder.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    string id = null;
                    try
                    {
                        connection.Open();

                        MySqlCommand cmd = connection.CreateCommand();
                        cmd.CommandText = "SELECT id, pwd, salt FROM users WHERE name=@name";
                        cmd.Parameters.AddWithValue("@name", textbox.Text);

                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (!rdr.HasRows)
                        {
                            Erreur(6);
                            etape--;
                            placeholder.Content = "Identifiant";
                            next_icon_png.RenderTransform = new RotateTransform(0);
                            textbox.Text = "";
                            passwordbox.Password = "";
                            passwordbox.Visibility = Visibility.Hidden;
                            placeholder.Visibility = Visibility.Visible;
                            textbox.Visibility = Visibility.Visible;

                        }
                        else
                        {
                            while (rdr.Read())
                            {
                                if (!ValidateMD5HashData(passwordbox.Password + rdr["salt"], rdr["pwd"].ToString()))
                                {
                                    Erreur(6);
                                    etape--;
                                    placeholder.Content = "Identifiant";
                                    next_icon_png.RenderTransform = new RotateTransform(0);
                                    textbox.Text = "";
                                    passwordbox.Password = "";
                                    passwordbox.Visibility = Visibility.Hidden;
                                    placeholder.Visibility = Visibility.Visible;
                                    textbox.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    id = rdr["id"].ToString();
                                    connexion = true;
                                }
                            }
                        }
                        rdr.Close();
                        connection.Close();
                    }
                    catch
                    {
                        Erreur(7);
                    }
                    if (connexion)
                    {
                        try
                        {
                            connection.Open();
                            MySqlCommand cmd = connection.CreateCommand();
                            cmd.CommandText = "UPDATE users SET remember_token = @r_t WHERE id = @id";

                            string rdm_str = Rdm_Str(128);
                            cmd.Parameters.AddWithValue("@r_t", rdm_str);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();

                            WriteData("db", "name", textbox.Text);
                            WriteData("db", "token", rdm_str);
                            connection.Close();
                            Connected();
                        }
                        catch
                        {
                            Erreur(7);
                        }
                    }
                }
            }
            else if (mode == 1)
            {
                if (etape == 0)
                {
                    if (textbox.Text.Length < 4 || textbox.Text.Length > 20)
                    {
                        Erreur(2);
                    }
                    else if (!Regex.IsMatch(textbox.Text, @"^[A-Za-záàâäãåçéèêëíìîïñóòôöõúùûüýÿæœÁÀÂÄÃÅÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸÆŒ\d_\.-]+$"))
                    {
                        Erreur(1);
                    }
                    else
                    {
                        try
                        {
                            connection.Open();

                            MySqlCommand cmd = connection.CreateCommand();
                            cmd.CommandText = "SELECT id FROM users WHERE name=@name";
                            cmd.Parameters.AddWithValue("@name", textbox.Text);

                            MySqlDataReader rdr = cmd.ExecuteReader();

                            if (rdr.HasRows)
                            {
                                Erreur(5);
                            }
                            else
                            {
                                etape++;
                                placeholder.Content = "Mot de passe";
                                textbox.Visibility = Visibility.Hidden;
                                passwordbox.Visibility = Visibility.Visible;
                                passwordbox.Focus();
                                placeholder.Visibility = Visibility.Visible;
                            }
                            rdr.Close();

                            connection.Close();
                        }
                        catch 
                        {
                            Erreur(7);
                        }
                    }
                }
                else if (etape == 1)
                {
                    if (passwordbox.Password.Length < 6 || passwordbox.Password.Length > 20)
                    {
                        Erreur(3);
                    }
                    else if (!Regex.IsMatch(passwordbox.Password, @"^[A-Za-záàâäãåçéèêëíìîïñóòôöõúùûüýÿæœÁÀÂÄÃÅÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸÆŒ\d_\#\!\$\?\+\*\/\.%-]+$"))
                    {
                        Erreur(1);
                    }
                    else
                    {
                        etape++;
                        placeholder.Content = "Répéter mdp";
                        pwd = passwordbox.Password;
                        passwordbox.Password = "";
                        next_icon_png.RenderTransform = new RotateTransform(90);
                    }
                }
                else
                {
                    if (passwordbox.Password != pwd)
                    {
                        Erreur(4);
                        etape--;
                        placeholder.Content = "Mot de passe";
                        passwordbox.Password = "";
                        next_icon_png.RenderTransform = new RotateTransform(0);
                    }
                    else
                    {
                        try
                        {
                            connection.Open();

                            MySqlCommand cmd = connection.CreateCommand();
                            cmd.CommandText = "INSERT INTO users (name, pwd, salt, remember_token) VALUES (@name, @pwd, @salt, @r_t)";

                            string rdm_str = Rdm_Str(128);
                            string salt = Rdm_Str(20);

                            cmd.Parameters.AddWithValue("@name", textbox.Text);
                            cmd.Parameters.AddWithValue("@pwd", GetMD5HashData(passwordbox.Password+salt));
                            cmd.Parameters.AddWithValue("@salt", salt);
                            cmd.Parameters.AddWithValue("@r_t", rdm_str);

                            cmd.ExecuteNonQuery();

                            connection.Close();

                            WriteData("db", "name", textbox.Text);
                            WriteData("db", "token", rdm_str);
                            Connected();
                        }
                        catch
                        {
                            Erreur(7);
                        }
                    }
                }
            }
        }

        private void ChangeMode() //Change de mode et actualise le formulaire
        {
            etape = 0; //init
            if (mode == 0)
            {
                mode = 1;
                sign_label.Content = "SE CONNECTER";
            }
            else
            {
                mode = 0;
                sign_label.Content = "CRÉER UN COMPTE";
            }
            placeholder.Content = "Identifiant";
            next_icon_png.RenderTransform = new RotateTransform(0);
            textbox.Text = "";
            passwordbox.Password = "";
            passwordbox.Visibility = Visibility.Hidden;
            placeholder.Visibility = Visibility.Visible;
            textbox.Visibility = Visibility.Visible;
        }

        private async void Erreur(int code) //Affiche les erreurs
        {
            sign_label.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF0000"));
            switch (code)
            {
                case 0:
                    sign_label.Content = "Identifiant incorrect";
                    break;
                case 1:
                    sign_label.Content = "Caractères non autorisés";
                    break;
                case 2:
                    sign_label.Content = "4 à 20 caractères nécessaires";
                    break;
                case 3:
                    sign_label.Content = "6 à 20 caractères nécessaires";
                    break;
                case 4:
                    sign_label.Content = "Mots de passe non identiques";
                    break;
                case 5:
                    sign_label.Content = "Identifiant déjà pris";
                    break;
                case 6:
                    sign_label.Content = "Identifiant/Mot de passe incorrect";
                    break;
                case 7:
                    sign_label.Content = "Problème serveur";
                    break;
                default:
                    sign_label.Content = "Erreur";
                    break;
            }
            await Task.Delay(2000);
            sign_label.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFEFEFEF"));
            if (mode == 0)
            {
                sign_label.Content = "CRÉER UN COMPTE";
            }
            else
            {
                sign_label.Content = "SE CONNECTER";
            }
        }

        private string Rdm_Str(int size)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZazertyuiopmlkjhgfdsqwxcvbn0123456789";
            return new string(Enumerable.Repeat(chars, size)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string ReadData(string name, string data)
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

        private void WriteData(string name, string data, string new_data)
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

        private void Connected()
        {
            SubWindow subwindow = new SubWindow();
            subwindow.Show();
            this.Close();
        }

        private string GetMD5HashData(string data) //Cryptage mdp
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

        private bool ValidateMD5HashData(string inputData, string storedHashData)
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

        private void close_png_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void next_icon_png_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EtapeSuivante();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EtapeSuivante();
            }
        }

        private void sign_label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeMode();
        }

        private void passwordbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordbox.Password != "")
            {
                placeholder.Visibility = Visibility.Hidden;
            }
            else
            {
                placeholder.Visibility = Visibility.Visible;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textbox.Text != "")
            {
                placeholder.Visibility = Visibility.Hidden;
            }
            else
            {
                placeholder.Visibility = Visibility.Visible;
            }
        }

        private void home_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
