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
using MySqlConnector;
using System.Data;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connection = "server=localhost;user=root;password=;database=mozi;";
        string sqlSzures = "";
        List<string> korhatar = new List<string>() {"Válassz...","KN","6","12","16","18"};
        List<string> mufaj = new List<string>() {"Válassz..."};
        public MainWindow()
        {
            InitializeComponent();
            CB_Korhatar.ItemsSource = korhatar;
            MufajFeltolt();
            CB_Mufaj.ItemsSource = mufaj;
            Betoltes();
            Nullazas();
        }

        private void Nullazas()
        {
            TB_Cim.Text = "";
            TB_Hossz.Text = "";
            CB_Korhatar.SelectedIndex = 0;
            CB_Mufaj.SelectedIndex = 0;
            RB_Osszes.IsChecked = true;

        }

        private void Betoltes()
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    string sqlBetolt = "SELECT * FROM filmek";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlBetolt, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    DG_Filmek.ItemsSource = dt.DefaultView;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Hiba az adatok elérésekor!");
                }
            }
        }

        private void MufajFeltolt()
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    string sqlMufaj = "SELECT DISTINCT mufaj FROM filmek";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlMufaj, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow sor in dt.Rows)
                    {
                        mufaj.Add(sor["mufaj"].ToString());
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Hiba az adatok elérésekor!");
                }
            }
        }

        private void BT_Hozzaad_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(TB_Cim.Text) || string.IsNullOrWhiteSpace(TB_Hossz.Text))
                    {
                        MessageBox.Show("A mező kitöltése kötelező!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (!SzamE(TB_Hossz.Text) || int.Parse(TB_Hossz.Text) < 0 || int.Parse(TB_Hossz.Text) > 300)
                    {
                        MessageBox.Show("A hossznak nagyobbnak kell lennie, mint 0", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (CB_Korhatar.SelectedIndex == 0 || CB_Mufaj.SelectedIndex == 0)
                    {
                        MessageBox.Show("A mező kiválasztása kötelező!", "Hiba!");
                        return;
                    }

                    conn.Open();

                    string sqlHozzaad = "INSERT INTO filmek(cim,hossz,korhatar,mufaj) VALUES (@c,@h,@k,@m)";
                    MySqlCommand cmdHozzaad = new MySqlCommand(sqlHozzaad, conn);
                    cmdHozzaad.Parameters.AddWithValue("@c",TB_Cim.Text);
                    cmdHozzaad.Parameters.AddWithValue("@h",TB_Hossz.Text);
                    cmdHozzaad.Parameters.AddWithValue("@k",CB_Korhatar.SelectedItem.ToString());
                    cmdHozzaad.Parameters.AddWithValue("@m",CB_Mufaj.SelectedItem.ToString());
                    int result = cmdHozzaad.ExecuteNonQuery();
                    if (result == 0)
                    {
                        MessageBox.Show("Nem történt feltöltés!", "Awww... :C", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        Betoltes();
                        Nullazas();
                        MessageBox.Show("A film hozzáadása sikeresen megtörtént!", "Yippee! :D", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hiba az adatok elérésekor!");
                }
            }
        }

        private void BT_Modosit_Click(object sender, RoutedEventArgs e)
        {
            if (DG_Filmek.SelectedItem == null)
            {
                MessageBox.Show("Nincs kiválasztva film!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView drv = DG_Filmek.SelectedItem as DataRowView;
            MessageBoxResult valasz = MessageBox.Show($"Biztosan szeretné módosítani a következő filmet?\n{drv["cim"]}\nA változtatások nem visszavonhatóak!", "Módosítás megerősítése", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
            if (valasz != MessageBoxResult.Yes)
            {
                Betoltes();
                Nullazas();
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(TB_Cim.Text) || string.IsNullOrWhiteSpace(TB_Hossz.Text))
                    {
                        MessageBox.Show("A mező kitöltése kötelező!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (!SzamE(TB_Hossz.Text) || int.Parse(TB_Hossz.Text) < 0 || int.Parse(TB_Hossz.Text) > 300)
                    {
                        MessageBox.Show("A hossznak nagyobbnak kell lennie, mint 0", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (CB_Korhatar.SelectedIndex == 0 || CB_Mufaj.SelectedIndex == 0)
                    {
                        MessageBox.Show("A mező kiválasztása kötelező!", "Hiba!");
                        return;
                    }

                    conn.Open();

                    string sqlModosit = "UPDATE filmek SET cim = @c, hossz = @h, korhatar = @k, mufaj = @m WHERE id = @id";
                    MySqlCommand cmdModosit = new MySqlCommand(sqlModosit, conn);
                    cmdModosit.Parameters.AddWithValue("@id", drv["id"]);
                    cmdModosit.Parameters.AddWithValue("@c", TB_Cim.Text);
                    cmdModosit.Parameters.AddWithValue("@h", TB_Hossz.Text);
                    cmdModosit.Parameters.AddWithValue("@k", CB_Korhatar.SelectedItem.ToString());
                    cmdModosit.Parameters.AddWithValue("@m", CB_Mufaj.SelectedItem.ToString());
                    int result = cmdModosit.ExecuteNonQuery();
                    if (result == 0)
                    {
                        MessageBox.Show("Nem történt módosítás!", "Awww... :C", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        Betoltes();
                        Nullazas();
                        MessageBox.Show("A film módosítása sikeresen megtörtént!", "Yippee? :/", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hiba az adatok elérésekor!");
                }
            }
        }

        private void BT_Torles_Click(object sender, RoutedEventArgs e)
        {
            if (DG_Filmek.SelectedItem == null)
            {
                MessageBox.Show("Nincs kiválasztva film!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DataRowView drv = DG_Filmek.SelectedItem as DataRowView;
            MessageBoxResult valasz = MessageBox.Show($"Biztosan ki szeretné törölni a következő filmet?\n{drv["cim"]}\nA törlés nem visszavonható!", "Törlés megerősítése", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
            if (valasz != MessageBoxResult.Yes)
            {
                return;
            }
            using ( MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();

                string sqlTorol = "DELETE FROM filmek WHERE id=@id";
                MySqlCommand cmdTorol = new MySqlCommand(sqlTorol, conn);
                cmdTorol.Parameters.AddWithValue("@id", drv["id"]);
                int result = cmdTorol.ExecuteNonQuery();
                if (result == 0)
                {
                    MessageBox.Show("Nem történt törlés!", "Awww... :C", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    Betoltes();
                    Nullazas();
                    MessageBox.Show("A film törlése sikeresen megtörtént!", "Yippee? :/", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BT_Frissit_Click(object sender, RoutedEventArgs e)
        {
            Betoltes();
            Nullazas();
        }

        private void DG_Filmek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DG_Filmek.SelectedItem == null)
            {
                return;
            }
            DataRowView drv = DG_Filmek.SelectedItem as DataRowView;
            TB_Cim.Text = drv["cim"].ToString();
            TB_Hossz.Text = drv["hossz"].ToString();
            CB_Korhatar.SelectedItem = drv["korhatar"].ToString();
            CB_Mufaj.SelectedItem = drv["mufaj"].ToString();
        }

        private bool SzamE(string szoveg)
        {
            return int.TryParse(szoveg, out int szam);
        }

        private void RB_Checked(object sender, RoutedEventArgs e)
        {
            if (DG_Filmek == null)
            {
                return;
            }
            RadioButton rb = sender as RadioButton;
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    if (rb.Content.ToString() == "Összes")
                    {
                        sqlSzures = "SELECT * FROM filmek";
                    }
                    else
                    {
                        sqlSzures = $"SELECT * FROM filmek WHERE korhatar = '{rb.Content.ToString()}'";
                    }
                    conn.Open();

                    MySqlDataAdapter adapter = new MySqlDataAdapter(sqlSzures, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    DG_Filmek.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hiba az adatok elérésekor!");
                }
            }
        }
    }
}
