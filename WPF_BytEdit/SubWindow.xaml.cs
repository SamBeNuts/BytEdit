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
using System.Windows.Shapes;

using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace WPF_BytEdit
{
    /// <summary>
    /// Logique d'interaction pour SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        public SubWindow()
        {
            InitializeComponent();

            //Chargement des projets
            Charge_Project();
        }

        /// <summary>
        /// Général :
        /// </summary>

        private void minimize_window(object sender, MouseButtonEventArgs e)
        {
            bytedit.WindowState = WindowState.Minimized;
        }

        private void maximize_window(object sender, MouseButtonEventArgs e)
        {
            if (bytedit.WindowState == WindowState.Maximized)
            {
                bytedit.WindowState = WindowState.Normal;
                maximize_btn_png.Source = new BitmapImage(new Uri("pack://application:,,/images/maximize_btn.png"));
            }
            else
            {
                bytedit.WindowState = WindowState.Maximized;
                maximize_btn_png.Source = new BitmapImage(new Uri("pack://application:,,/images/normal_btn.png"));
            }
        }

        private void close_window(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void move_window(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            try
            {
                this.DragMove();
            }
            catch
            {

            }
        }

        /// <summary>
        /// DASHBOARD :
        /// </summary>

        /// <summary>
        /// Général :
        /// </summary>

        private void Disconnect()
        {
            App.WriteData("db", "token", "");
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }

        private void Call_Disconnect(object sender, MouseButtonEventArgs e)
        {
            Disconnect();
        }

        private void go_to_editor(object sender, MouseButtonEventArgs e)
        {
            edit.Visibility = Visibility.Visible;
            board.Visibility = Visibility.Hidden;
        }

        private void ContextMenuLeftClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image)
            {
                ((Image)sender).ContextMenu.IsOpen = true;
            }
        }

        private void Erreur(int code) //Affiche les erreurs
        {
            switch (code)
            {
                case 0:
                    MessageBox.Show("Problème serveur", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 1:
                    MessageBox.Show("6 à 50 caractères autorisés pour le titre", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 2:
                    MessageBox.Show("Caractères non autorisés pour le titre", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 3:
                    MessageBox.Show("300 caractères maximum pour la description", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 4:
                    MessageBox.Show("1 à 50 caractères autorisés", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 5:
                    MessageBox.Show("Ce nom est déjà pris", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                default:
                    MessageBox.Show("Une erreur est survenue", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }

        /// <summary>
        /// Arborescence projet :
        /// </summary>

        //Pour chaque projet appelle la fonction New_Project_In_List()
        private void Charge_Project() 
        {
            try
            {
                App.connection.Open();

                MySqlCommand cmd = App.connection.CreateCommand();

                cmd.CommandText = "SELECT projects.name AS name, projects.id AS id FROM projects INNER JOIN link_projects ON link_projects.project_id = projects.id WHERE link_projects.user_id = @user_id ORDER BY projects.name";
                cmd.Parameters.AddWithValue("@user_id", App.id_);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    New_Project_In_List(rdr["name"].ToString(), rdr["id"].ToString());
                }
                rdr.Close();

                App.connection.Close();
            }
            catch
            {
                Erreur(0);
            }
        }

        //Rajoute un projet dans l'arborescence projet
        private void New_Project_In_List(string name, string id) 
        {
            Grid grid = new Grid();
            grid.Height = 40;
            grid.MouseDown += new MouseButtonEventHandler(Project_Focus);
            grid.Tag = id;
            ColumnDefinition grid_column1 = new ColumnDefinition();
            grid_column1.Width = new GridLength(5);
            ColumnDefinition grid_column2 = new ColumnDefinition();
            grid_column2.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition grid_column3 = new ColumnDefinition();
            grid_column3.Width = new GridLength(40);
            ColumnDefinition grid_column4 = new ColumnDefinition();
            grid_column4.Width = new GridLength(5);
            Rectangle r1 = new Rectangle();
            r1.Fill = App.black;
            Rectangle r2 = new Rectangle();
            r2.Fill = App.black;
            r2.Visibility = Visibility.Hidden;
            Ellipse e1 = new Ellipse();
            e1.Fill = Brushes.Green;
            e1.Height = 7;
            e1.Width = 7;
            e1.Margin = new Thickness(-20, 0, 0, 0);
            Ellipse e2 = new Ellipse();
            e2.Fill = Brushes.Blue;
            e2.Height = 7;
            e2.Width = 7;
            Ellipse e3 = new Ellipse();
            e3.Fill = Brushes.Red;
            e3.Height = 7;
            e3.Width = 7;
            e3.Margin = new Thickness(20, 0, 0, 0);
            Image i = new Image();
            i.Source = new BitmapImage(new Uri("pack://application:,,/images/settings_icon_yellow.png"));
            i.Height = 20;
            i.Width = 20;
            i.Visibility = Visibility.Hidden;
            Run l_run = new Run() { Text = name };
            l_run.Typography.Capitals = FontCapitals.SmallCaps;
            Label l = new Label();
            l.Content = l_run;
            l.Foreground = App.black;
            l.FontSize = 22;
            l.FontWeight = FontWeights.Bold;
            l.Padding = new Thickness(0);
            l.Margin = new Thickness(5,0,0,0);
            l.VerticalAlignment = VerticalAlignment.Center;
            grid.ColumnDefinitions.Add(grid_column1);
            grid.ColumnDefinitions.Add(grid_column2);
            grid.ColumnDefinitions.Add(grid_column3);
            grid.ColumnDefinitions.Add(grid_column4);
            grid.Children.Add(r1);
            grid.Children.Add(r2);
            grid.Children.Add(e1);
            grid.Children.Add(e2);
            grid.Children.Add(e3);
            grid.Children.Add(i);
            grid.Children.Add(l);
            Grid.SetColumn(r1, 0);
            Grid.SetColumn(l, 1);
            Grid.SetColumn(e1, 2);
            Grid.SetColumn(e2, 2);
            Grid.SetColumn(e3, 2);
            Grid.SetColumn(i, 2);
            Grid.SetColumn(r2, 3);
            project_list.Children.Add(grid);
        }

        //Gère le focus d'un projet
        private void Project_Focus(object sender, MouseButtonEventArgs e)
        {
            Grid grid_focus = (Grid)sender;
            foreach (Grid grid in project_list.Children)
            {
                if (grid != grid_focus)
                {
                    grid.Background = App.white;
                    grid.Children[0].Visibility = Visibility.Visible;
                    grid.Children[1].Visibility = Visibility.Hidden;
                    grid.Children[2].Visibility = Visibility.Visible;
                    grid.Children[3].Visibility = Visibility.Visible;
                    grid.Children[4].Visibility = Visibility.Visible;
                    grid.Children[5].Visibility = Visibility.Hidden;
                }
                else
                {
                    project_view.Tag = grid.Tag;
                    grid.Background = App.or;
                    grid.Children[0].Visibility = Visibility.Hidden;
                    grid.Children[1].Visibility = Visibility.Visible;
                    grid.Children[2].Visibility = Visibility.Hidden;
                    grid.Children[3].Visibility = Visibility.Hidden;
                    grid.Children[4].Visibility = Visibility.Hidden;
                    grid.Children[5].Visibility = Visibility.Visible;
                }
            }
            file_view.Visibility = Visibility.Visible;
        }

        private void Open_Project_Creation(object sender, MouseButtonEventArgs e)
        {
            project_creation_box.Visibility = Visibility.Visible;
        }

        private void Create_Project(object sender, MouseButtonEventArgs e)
        {
            if (name_project.Text.Length < 6 || name_project.Text.Length > 50)
            {
                Erreur(1);
            }
            else if (!Regex.IsMatch(name_project.Text, @"^[A-Za-záàâäãåçéèêëíìîïñóòôöõúùûüýÿæœÁÀÂÄÃÅÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸÆŒ\d_\.-]+$"))
            {
                Erreur(2);
            }
            else if (description_project.Text.Length > 300)
            {
                Erreur(3);
            }
            else
            {
                try
                {
                    App.connection.Open();

                    string key = App.Rdm_Str(128), project_id = null;

                    MySqlCommand cmd = App.connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO projects (name, description, share_mode, clef, version_current) VALUES (@name, @description, @share_mode, @key, @version_current)";
                    cmd.Parameters.AddWithValue("@name", name_project.Text);
                    cmd.Parameters.AddWithValue("@description", description_project.Text);
                    if (share_mode.SelectedIndex == 0)
                    {
                        cmd.Parameters.AddWithValue("@share_mode", null);
                    }
                    else if (share_mode.SelectedIndex == 1)
                    {
                        cmd.Parameters.AddWithValue("@share_mode", 0);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@share_mode", 1);
                    }
                    cmd.Parameters.AddWithValue("@key", key);
                    cmd.Parameters.AddWithValue("@version_current", 1);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT id FROM projects WHERE name=@name2 AND clef=@key2";
                    cmd.Parameters.AddWithValue("@name2", name_project.Text);
                    cmd.Parameters.AddWithValue("@key2", key);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        project_id = rdr["id"].ToString();
                    }
                    rdr.Close();

                    cmd.CommandText = "INSERT INTO link_projects (user_id, project_id, last_open, admin) VALUES (@user_id, @project_id, @last_open, @admin)";
                    cmd.Parameters.AddWithValue("@user_id", App.id_);
                    cmd.Parameters.AddWithValue("@project_id", project_id);
                    cmd.Parameters.AddWithValue("@last_open", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@admin", 1);
                    cmd.ExecuteNonQuery();

                    App.connection.Close();
                    New_Project_In_List(name_project.Text, project_id);
                    CloseProjectCreation(sender, e);
                }
                catch
                {
                    Erreur(0);
                }
            }
        }

        private void CloseProjectCreation(object sender, MouseButtonEventArgs e)
        {
            name_project.Text = "";
            description_project.Text = "";
            share_mode.SelectedIndex = 0;
            project_creation_box.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Arborescence fichier :
        /// </summary>

        private void Create_File(object sender, MouseButtonEventArgs e)
        {
            if (name_file.Text.Length < 1 || name_file.Text.Length > 50)
            {
                Erreur(4);
            }
            else if (!Regex.IsMatch(name_file.Text, @"^[A-Za-záàâäãåçéèêëíìîïñóòôöõúùûüýÿæœÁÀÂÄÃÅÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸÆŒ\d_\.-]+$"))
            {
                Erreur(2);
            }
            else
            {
                bool exist = true;
                try
                {
                    App.connection.Open();

                    MySqlCommand cmd = App.connection.CreateCommand();
                    cmd.CommandText = "SELECT id FROM files WHERE name=@name AND project_id=@project_id AND folder=@folder";
                    cmd.Parameters.AddWithValue("@name", name_file.Text);
                    cmd.Parameters.AddWithValue("@project_id", project_view.Tag);
                    cmd.Parameters.AddWithValue("@folder", 0);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        Erreur(5);
                    }
                    else
                    {
                        exist = false;
                    }
                    rdr.Close();

                    App.connection.Close();
                }
                catch
                {
                    Erreur(0);
                }
                if (!exist)
                {
                    try
                    {
                        App.connection.Open();

                        string file_id = null;

                        MySqlCommand cmd = App.connection.CreateCommand();
                        cmd.CommandText = "INSERT INTO files (name, project_id, deleted, folder, folder_id, file_version_id) VALUES (@name, @project_id, @deleted, @folder, @folder_id, @file_version_id)";
                        cmd.Parameters.AddWithValue("@name", name_file.Text);
                        cmd.Parameters.AddWithValue("@project_id", project_view.Tag);
                        cmd.Parameters.AddWithValue("@deleted", 0);
                        cmd.Parameters.AddWithValue("@folder", 0);
                        if (folder.SelectedIndex == 0)
                        {
                            cmd.Parameters.AddWithValue("@folder_id", null);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@folder_id", ((ComboBoxItem)folder.SelectedItem).Tag);
                        }
                        cmd.Parameters.AddWithValue("@file_version_id", 1);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT id FROM files WHERE name=@name2 AND project_id=@project_id2";
                        cmd.Parameters.AddWithValue("@name2", name_file.Text);
                        cmd.Parameters.AddWithValue("@project_id2", project_view.Tag);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            file_id = rdr["id"].ToString();
                        }
                        rdr.Close();

                        cmd.CommandText = "INSERT INTO right_files (user_id, file_id, share_right) VALUES (@user_id, @file_id, @share_right)";
                        cmd.Parameters.AddWithValue("@user_id", App.id_);
                        cmd.Parameters.AddWithValue("@file_id", file_id);
                        cmd.Parameters.AddWithValue("@share_right", null);
                        cmd.ExecuteNonQuery();

                        App.connection.Close();
                        //New_File_In_List(name_file.Text, file_id);
                        CloseFileCreation(sender, e);
                    }
                    catch
                    {
                        Erreur(0);
                    }
                }
            }
        }

        private void Create_Folder(object sender, MouseButtonEventArgs e)
        {
            if (name_folder.Text.Length < 1 || name_folder.Text.Length > 50)
            {
                Erreur(4);
            }
            else if (!Regex.IsMatch(name_folder.Text, @"^[A-Za-záàâäãåçéèêëíìîïñóòôöõúùûüýÿæœÁÀÂÄÃÅÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸÆŒ\d_\.\s-]+$"))
            {
                Erreur(2);
            }
            else
            {
                bool exist = true;
                try
                {
                    App.connection.Open();

                    MySqlCommand cmd = App.connection.CreateCommand();
                    cmd.CommandText = "SELECT id FROM files WHERE name=@name AND project_id=@project_id AND folder=@folder";
                    cmd.Parameters.AddWithValue("@name", name_folder.Text);
                    cmd.Parameters.AddWithValue("@project_id", project_view.Tag);
                    cmd.Parameters.AddWithValue("@folder", 1);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        Erreur(5);
                    }
                    else
                    {
                        exist = false;
                    }
                    rdr.Close();

                    App.connection.Close();
                }
                catch
                {
                    Erreur(0);
                }
                if (!exist)
                {
                    try
                    {
                        App.connection.Open();

                        MySqlCommand cmd = App.connection.CreateCommand();
                        cmd.CommandText = "INSERT INTO files (name, project_id, deleted, folder, folder_id, file_version_id) VALUES (@name, @project_id, @deleted, @folder, @folder_id, @file_version_id)";
                        cmd.Parameters.AddWithValue("@name", name_folder.Text);
                        cmd.Parameters.AddWithValue("@project_id", project_view.Tag);
                        cmd.Parameters.AddWithValue("@deleted", 0);
                        cmd.Parameters.AddWithValue("@folder", 1);
                        if (folder2.SelectedIndex == 0)
                        {
                            cmd.Parameters.AddWithValue("@folder_id", null);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@folder_id", ((ComboBoxItem)folder2.SelectedItem).Tag);
                        }
                        cmd.Parameters.AddWithValue("@file_version_id", 1);
                        cmd.ExecuteNonQuery();

                        App.connection.Close();
                        //New_Folder_In_List(name_folder.Text);
                        CloseFolderCreation(sender, e);
                    }
                    catch
                    {
                        Erreur(0);
                    }
                }
            }
        }

        private void CloseFileCreation(object sender, MouseButtonEventArgs e)
        {
            name_file.Text = "";
            folder.Items.Clear();
            file_creation_box.Visibility = Visibility.Hidden;
        }

        private void CloseFolderCreation(object sender, MouseButtonEventArgs e)
        {
            name_folder.Text = "";
            folder2.Items.Clear();
            folder_creation_box.Visibility = Visibility.Hidden;
        }

        private void MenuItem_File_Click(object sender, System.EventArgs e)
        {
            try
            {
                App.connection.Open();

                MySqlCommand cmd = App.connection.CreateCommand();
                cmd.CommandText = "SELECT name, id FROM files WHERE project_id=@project_id AND folder=@folder ORDER BY name";
                cmd.Parameters.AddWithValue("@project_id", project_view.Tag);
                cmd.Parameters.AddWithValue("@folder", 1);
                MySqlDataReader rdr = cmd.ExecuteReader();
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = "Aucun";
                cbi.IsSelected = true;
                folder.Items.Add(cbi);
                while (rdr.Read())
                {
                    cbi = new ComboBoxItem();
                    cbi.Content = rdr["name"];
                    cbi.Tag = rdr["id"];
                    folder.Items.Add(cbi);
                }
                rdr.Close();

                file_creation_box.Visibility = Visibility.Visible;
                App.connection.Close();
            }
            catch
            {
                Erreur(0);
            }
        }

        private void MenuItem_Folder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App.connection.Open();

                MySqlCommand cmd = App.connection.CreateCommand();
                cmd.CommandText = "SELECT name, id FROM files WHERE project_id=@project_id AND folder=@folder ORDER BY name";
                cmd.Parameters.AddWithValue("@project_id", project_view.Tag);
                cmd.Parameters.AddWithValue("@folder", 1);
                MySqlDataReader rdr = cmd.ExecuteReader();
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = "Aucun";
                cbi.IsSelected = true;
                folder2.Items.Add(cbi);
                while (rdr.Read())
                {
                    cbi = new ComboBoxItem();
                    cbi.Content = rdr["name"];
                    cbi.Tag = rdr["id"];
                    folder2.Items.Add(cbi);
                }
                rdr.Close();
                folder_creation_box.Visibility = Visibility.Visible;
                App.connection.Close();
            }
            catch
            {
                Erreur(0);
            }
        }

        /// <summary>
        /// EDITEUR :
        /// </summary>

        /// <summary>
        /// Général :
        /// </summary>

        private void back_to_board(object sender, MouseButtonEventArgs e)
        {
            edit.Visibility = Visibility.Hidden;
            board.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Barre des tâches
        /// </summary>

        //En cours
        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            fichier_panel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF333333"));
        }

        //En cours
        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            fichier_panel.Background = App.black;
        }
    }
}
