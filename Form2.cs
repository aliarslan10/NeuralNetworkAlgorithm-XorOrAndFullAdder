using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MExcel = Microsoft.Office.Interop.Excel;

namespace YapaySinirAgi
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            agiKur();
        }

        float ogrenmeKatsayisi = 0.9f;
        float momentum = 0.8f; //önerilen: 0.6 ile 0.8 arası.
        int iterasyonSayisi = 1000;

        float agirlik1_4, agirlik1_5;
        float agirlik2_4, agirlik2_5;
        float agirlik3_4, agirlik3_5;
        float agirlik4_6, agirlik5_6;
        float agirlik4_7, agirlik5_7;
        float agirlikE1_4, agirlikE1_5;
        float agirlikE2_6, agirlikE2_7;

        float degisim1_4, degisim1_5;
        float degisim2_4, degisim2_5;
        float degisim3_4, degisim3_5;
        float degisim4_6, degisim5_6;
        float degisim4_7, degisim5_7;
        float degisimE1_4, degisimE1_5;
        float degisimE2_6, degisimE2_7;

        float noron1, noron2, noron3, noron4, noron5, noron6, noron7;
        float esik1 = 1.0f, esik2 = 1.0f;

        ListBox listboxHataCikti = new ListBox();
        ListBox listboxHataElde = new ListBox();
        ListBox listboxAgirlik1_4 = new ListBox();
        ListBox listboxAgirlik1_5 = new ListBox();
        ListBox listboxAgirlik2_4 = new ListBox();
        ListBox listboxAgirlik2_5 = new ListBox();
        ListBox listboxAgirlik3_4 = new ListBox();
        ListBox listboxAgirlik3_5 = new ListBox();
        ListBox listboxAgirlik4_6 = new ListBox();
        ListBox listboxAgirlik5_6 = new ListBox();
        ListBox listboxAgirlik4_7 = new ListBox();
        ListBox listboxAgirlik5_7 = new ListBox();
        ListBox listboxAgirlikE1_4 = new ListBox();
        ListBox listboxAgirlikE1_5 = new ListBox();
        ListBox listboxAgirlikE2_6 = new ListBox();
        ListBox listboxAgirlikE2_7 = new ListBox();

        Random uret = new Random(); //her defasında farklı değerler üretmesi için public olması gerekiyor.
        private float rastgeleAgirlikUret()
        {
            return (float)uret.Next(1, 999) / 1000.0f;
        }

        private void agiKur()
        {
            agirlik1_4 = rastgeleAgirlikUret();
            lblW1_4.Text = agirlik1_4.ToString();

            agirlik1_5 = rastgeleAgirlikUret();
            lblW1_5.Text = agirlik1_5.ToString();

            agirlik2_4 = rastgeleAgirlikUret();
            lblW2_4.Text = agirlik2_4.ToString();

            agirlik2_5 = rastgeleAgirlikUret();
            lblW2_5.Text = agirlik2_5.ToString();

            agirlik3_4 = rastgeleAgirlikUret();
            lblW3_4.Text = agirlik3_4.ToString();

            agirlik3_5 = rastgeleAgirlikUret();
            lblW3_5.Text = agirlik3_5.ToString();

            agirlik4_6 = rastgeleAgirlikUret();
            lblW4_6.Text = agirlik4_6.ToString();

            agirlik5_6 = rastgeleAgirlikUret();
            lblW5_6.Text = agirlik5_6.ToString();

            agirlikE1_4 = rastgeleAgirlikUret();
            lblWe1_4.Text = agirlikE1_4.ToString();

            agirlikE1_5 = rastgeleAgirlikUret();
            lblWe1_5.Text = agirlikE1_5.ToString();

            agirlikE2_6 = rastgeleAgirlikUret();
            lblWe2_6.Text = agirlikE2_6.ToString();

            agirlik4_7 = rastgeleAgirlikUret();
            lblW4_7.Text = agirlik4_7.ToString();

            agirlik5_7 = rastgeleAgirlikUret();
            lblW5_7.Text = agirlik5_7.ToString();

            agirlikE2_7 = rastgeleAgirlikUret();
            lblWe2_7.Text = agirlikE2_7.ToString();

            degisim1_4 = 0; degisim1_5 = 0;
            degisim2_4 = 0; degisim2_5 = 0;
            degisim3_4 = 0; degisim3_5 = 0;
            degisim4_6 = 0; degisim5_6 = 0;
            degisim4_7 = 0; degisim5_7 = 0;
            degisimE1_4 = 0; degisimE1_5 = 0;
            degisimE2_6 = 0; degisimE2_7 = 0;

            lblN4Cikis.Text = "0.000000";
            lblN5Cikis.Text = "0.000000";
            lblN6Cikis.Text = "0.000000";
            lblN7Cikis.Text = "0.000000";

            listboxTemizle();
            butonAgPasifEgitimAktif();

        }

        private float aktivasyon(float x) //sigmoid fonksiyonu
        {
            return (float)(1.0f / (1.0f + Math.Exp(-x)));
        }

 
        private void ileriHesaplamaIslemleri(float giris1, float giris2, float giris3)
        {
            noron1 = (float)giris1;
            noron2 = (float)giris2;
            noron3 = (float)giris3;

            noron4 = (noron1 * agirlik1_4) + (noron2 * agirlik2_4) + (noron3 * agirlik3_4) + (esik1 * agirlikE1_4);
            noron4 = aktivasyon(noron4);

            noron5 = (noron1 * agirlik1_5) + (noron2 * agirlik2_5) + (noron3 * agirlik3_5) + (esik1 * agirlikE1_5);
            noron5 = aktivasyon(noron5);

            noron6 = (noron4 * agirlik4_6) + (noron5 * agirlik5_6) + (esik2 * agirlikE2_6);
            noron6 = aktivasyon(noron6);

            noron7 = (noron4 * agirlik4_7) + (noron5 * agirlik5_7) + (esik2 * agirlikE2_7);
            noron7 = aktivasyon(noron7);

            lblN4Cikis.Text = noron4.ToString();
            lblN5Cikis.Text = noron5.ToString();
            lblN6Cikis.Text = noron6.ToString();
            lblN7Cikis.Text = noron7.ToString();

            lblGiris1.Text = noron1.ToString();
            lblGiris2.Text = noron2.ToString();
            lblGiris3.Text = noron3.ToString();

            listboxAgirlik1_4.Items.Add(agirlik1_4); listboxAgirlik1_5.Items.Add(agirlik1_5);
            listboxAgirlik2_4.Items.Add(agirlik2_4); listboxAgirlik2_5.Items.Add(agirlik2_5);
            listboxAgirlik3_4.Items.Add(agirlik3_4); listboxAgirlik3_5.Items.Add(agirlik3_5);
            listboxAgirlik4_6.Items.Add(agirlik4_6); listboxAgirlik5_6.Items.Add(agirlik5_6);
            listboxAgirlik4_7.Items.Add(agirlik4_7); listboxAgirlik5_7.Items.Add(agirlik5_7);
            listboxAgirlikE1_4.Items.Add(agirlikE1_4); listboxAgirlikE1_5.Items.Add(agirlikE1_5);
            listboxAgirlikE2_6.Items.Add(agirlikE2_6); listboxAgirlikE2_7.Items.Add(agirlikE2_7);
        }

        private void geriHesaplamaIslemleri(float giris1, float giris2, float giris3, float istenenCikis, float elde) //backpropagation ile eğitim
        {
            ileriHesaplamaIslemleri(giris1, giris2, giris3);
            Application.DoEvents();

            //hata hesapları: (önce noron6'nın çıkışına göre hata hesabı yapıyoruz)
            float hataN6 = (noron6) * (1 - noron6) * (istenenCikis - noron6); //çıkış1'deki hata hesabı
            float hataN7 = (noron7) * (1 - noron7) * (elde - noron7); //çıkış2='elde'deki hata hesabı
            float hataN5 = noron5 * (1 - noron5) * ((hataN6 * agirlik5_6) + (hataN7 * agirlik5_7)); // ara katmandaki hata hesabı
            float hataN4 = noron4 * (1 - noron4) * ((hataN6 * agirlik4_6) + (hataN7 * agirlik4_7));//ara katmandaki hata hesabı

            //Listbox Doldur Hata:
            listboxHataCikti.Items.Add(Math.Abs(hataN6));
            listboxHataElde.Items.Add(Math.Abs(hataN7));

            //ağırlık değişimlerinin hesaplanması:
            degisim1_4 = ogrenmeKatsayisi * hataN4 * noron1 + (momentum * degisim1_4);
            agirlik1_4 = agirlik1_4 + degisim1_4;
            lblW1_4.Text = agirlik1_4.ToString();

            degisim1_5 = ogrenmeKatsayisi * hataN5 * noron1 + (momentum * degisim1_5);
            agirlik1_5 = agirlik1_5 + degisim1_5;
            lblW1_5.Text = agirlik1_5.ToString();

            degisim2_4 = ogrenmeKatsayisi * hataN4 * noron2 + (momentum * degisim2_4);
            agirlik2_4 = agirlik2_4 + degisim2_4;
            lblW2_4.Text = agirlik2_4.ToString();

            degisim2_5 = ogrenmeKatsayisi * hataN5 * noron2 + (momentum * degisim2_5);
            agirlik2_5 = agirlik2_5 + degisim2_5;
            lblW2_5.Text = agirlik2_5.ToString();

            degisim3_4 = ogrenmeKatsayisi * hataN4 * noron3 + (momentum * degisim3_4);
            agirlik3_4 = agirlik3_4 + degisim3_4;
            lblW3_4.Text = agirlik3_4.ToString();

            degisim3_5 = ogrenmeKatsayisi * hataN5 * noron3 + (momentum * degisim3_5);
            agirlik3_5 = agirlik3_5 + degisim3_5;
            lblW3_5.Text = agirlik3_5.ToString();

            degisimE1_4 = ogrenmeKatsayisi * hataN4 * esik1 + (momentum * degisimE1_4);
            agirlikE1_4 = agirlikE1_4 + degisimE1_4;
            lblWe1_4.Text = agirlikE1_4.ToString();

            degisimE1_5 = ogrenmeKatsayisi * hataN5 * esik1 + (momentum * degisimE1_5);
            agirlikE1_5 = agirlikE1_5 + degisimE1_5;
            lblWe1_5.Text = agirlikE1_5.ToString();

            degisim4_6 = ogrenmeKatsayisi * hataN6 * noron4 + (momentum * degisim4_6);
            agirlik4_6 = agirlik4_6 + degisim4_6;
            lblW4_6.Text = agirlik4_6.ToString();

            degisim5_6 = ogrenmeKatsayisi * hataN6 * noron5 + (momentum * degisim5_6);
            agirlik5_6 = agirlik5_6 + degisim5_6;
            lblW5_6.Text = agirlik5_6.ToString();

            degisimE2_6 = ogrenmeKatsayisi * hataN6 * esik2 + (momentum * degisimE2_6);
            agirlikE2_6 = agirlikE2_6 + degisimE2_6;
            lblWe2_6.Text = agirlikE2_6.ToString();

            degisim4_7 = ogrenmeKatsayisi * hataN7 * noron4 + (momentum * degisim4_7);
            agirlik4_7 = agirlik4_7 + degisim4_7;
            lblW4_7.Text = agirlik4_7.ToString();
            
            degisim5_7 = ogrenmeKatsayisi * hataN7 * noron5 + (momentum * degisim5_7);
            agirlik5_7 = agirlik5_7 + degisim5_7;
            lblW5_7.Text = agirlik5_7.ToString();

            degisimE2_7 = ogrenmeKatsayisi * hataN7 * esik2 + (momentum * degisimE2_7);
            agirlikE2_7 = agirlikE2_7 + degisimE2_7;
            lblWe2_7.Text = agirlikE2_7.ToString();
        }

        private void btnEgit_Click(object sender, EventArgs e)
        {
            btnEgit.Enabled = false;
            float giris1, giris2, giris3, cikis = 0.0f, elde = 0.0f;
            giris1 = float.Parse(tbGiris1.Text);
            giris2 = float.Parse(tbGiris2.Text);
            giris3 = float.Parse(tbGiris3.Text);


            if (giris1 == 0 && giris2 == 0 && giris3 == 0)
            {
                cikis = 0; elde = 0;
            }
                
            if (giris1 == 0 && giris2 == 0 && giris3 == 1)
            {
                cikis = 1; elde = 0;
            }

            if (giris1 == 0 && giris2 == 1 && giris3 == 0)
            {
                cikis = 1; elde = 0;
            }

            if (giris1 == 0 && giris2 == 1 && giris3 == 1)
            {
                cikis = 0; elde = 1;
            }

            if (giris1 == 1 && giris2 == 0 && giris3 == 0)
            {
                cikis = 1; elde = 0;
            }

            if (giris1 == 1 && giris2 == 0 && giris3 == 1)
            {
                cikis = 0; elde = 1;
            }

            if (giris1 == 1 && giris2 == 1 && giris3 == 0)
            {
                cikis = 0; elde = 1;
            }

            if (giris1 == 1 && giris2 == 1 && giris3 == 1)
            {
                cikis = 1; elde = 1;
            }

            for (int i = 0; i <= iterasyonSayisi; i++)
            {
                geriHesaplamaIslemleri(giris1, giris2, giris3, cikis, elde);
            }

            excelButonlariAktif();
            butonEgitimPasifAgAktif();
        }

        private void btnAgiKur_Click(object sender, EventArgs e)
        {
            agiKur();
            tbGiris1.Text = "";
            tbGiris2.Text = "";
            tbGiris3.Text = "";
        }

        private void butonAgPasifEgitimAktif()
        {
            btnAgKur.Enabled = false;
            btnEgit.Enabled = true;
            excelButonlariPasifEt();
        }

        private void butonEgitimPasifAgAktif()
        {
            btnAgKur.Enabled = true;
            btnEgit.Enabled = false;
        }

        private void excelButonlariAktif()
        {
            btnHataExcel.Enabled = true;
            btnCiktiExcel.Enabled = true;
        }

        private void excelButonlariPasifEt()
        {
            btnHataExcel.Enabled = false;
            btnCiktiExcel.Enabled = false;
        }


        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Graphics grafik = this.CreateGraphics();
            Pen kalem1 = new Pen(Color.Blue, 2); //şekli çizmek için gerekli.
            Pen kalem2 = new Pen(Color.Gray, 2);
            Pen kalem3 = new Pen(Color.Black, 1);
            Brush firca = new SolidBrush(Color.Black); //string yazmak için mutlaka gerekiyor. parametre olarak.

            //Ellipse çizimleri
            grafik.DrawEllipse(kalem1, 20, 20, 75, 75);
            grafik.DrawString("N1", new Font("Arial", 10, FontStyle.Regular), firca, 50, 40);

            grafik.DrawEllipse(kalem1, 20, 120, 75, 75);
            grafik.DrawString("N2", new Font("Arial", 10, FontStyle.Regular), firca, 50, 140);

            grafik.DrawEllipse(kalem1, 20, 220, 75, 75);
            grafik.DrawString("N3", new Font("Arial", 10, FontStyle.Regular), firca, 50, 240);

            grafik.DrawEllipse(kalem2, 100, 300, 75, 75);
            grafik.DrawString("E1", new Font("Arial", 10, FontStyle.Regular), firca, 130, 320);

            grafik.DrawEllipse(kalem1, 270, 60, 75, 75);
            grafik.DrawString("N4", new Font("Arial", 10, FontStyle.Regular), firca, 300, 80);

            grafik.DrawEllipse(kalem1, 270, 180, 75, 75);
            grafik.DrawString("N5", new Font("Arial", 10, FontStyle.Regular), firca, 300, 200);

            grafik.DrawEllipse(kalem2, 300, 300, 75, 75);
            grafik.DrawString("E2", new Font("Arial", 10, FontStyle.Regular), firca, 330, 320);

            grafik.DrawEllipse(kalem1, 450, 60, 75, 75);
            grafik.DrawString("N6", new Font("Arial", 10, FontStyle.Regular), firca, 480, 80);

            grafik.DrawEllipse(kalem1, 450, 175, 75, 75);
            grafik.DrawString("N7", new Font("Arial", 10, FontStyle.Regular), firca, 480, 195);


            //Kalem çizimleri
            grafik.DrawLine(kalem3, 95, 50, 270, 90);    //n1-n4
            grafik.DrawLine(kalem3, 95, 50, 270, 210);   //n1-n5
            grafik.DrawLine(kalem3, 95, 150, 270, 90);   //n2-n4
            grafik.DrawLine(kalem3, 95, 150, 270, 210);  //n2-n5
            grafik.DrawLine(kalem3, 95, 250, 270, 90);   //n3-n4
            grafik.DrawLine(kalem3, 95, 250, 270, 210);  //n3-n5

            grafik.DrawLine(kalem3, 140, 300, 270, 90);  //e1-n4
            grafik.DrawLine(kalem3, 140, 300, 270, 210); //e1-n5

            grafik.DrawLine(kalem3, 345, 100, 450, 100);  //n4-n6
            grafik.DrawLine(kalem3, 345, 210, 450, 100);  //n5-n6
            grafik.DrawLine(kalem3, 350, 300, 450, 100);  //e2-n6

            grafik.DrawLine(kalem3, 345, 100, 450, 210);  //n4-n7
            grafik.DrawLine(kalem3, 345, 210, 450, 210);  //n5-n7
            grafik.DrawLine(kalem3, 350, 300, 450, 210);  //e2-n7
        }

        private void tbKontrol_KeyPress(object sender, KeyPressEventArgs e)
        {

            if ((e.KeyChar).ToString() == "0" || (e.KeyChar).ToString() == "1" || e.KeyChar == (char)8)
            {
                e.Handled = false;
            }

            else
            {
                e.Handled = true;
                MessageBox.Show("Sadece 0 ve 1 değerleri girilebilir", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnHataExcel_Click(object sender, EventArgs e)
        {
            MExcel.Application excel_uygulamasi = new MExcel.Application();
            excel_uygulamasi.Visible = true;

            MExcel.Workbook excel_yazdir = excel_uygulamasi.Workbooks.Add(true);
            MExcel.Worksheet excel_sayfasi = (MExcel.Worksheet)excel_uygulamasi.Sheets[1];
           
            for (int i = 1; i <= iterasyonSayisi; i++)
            {
                try
                {
                        MExcel.Range hataCiktiYazdir = (MExcel.Range)excel_sayfasi.Cells[i, 1];
                        hataCiktiYazdir.Value2 = listboxHataCikti.Items[i - 1];

                        MExcel.Range hataEldeYazdir = (MExcel.Range)excel_sayfasi.Cells[i, 2];
                        hataEldeYazdir.Value2 = listboxHataElde.Items[i - 1];
                }

                catch
                {

                }

                finally
                {
                    MExcel.Range hataCiktiYazdir = (MExcel.Range)excel_sayfasi.Cells[i, 1];
                    hataCiktiYazdir.Value2 = listboxHataCikti.Items[i - 1];

                    MExcel.Range hataEldeYazdir = (MExcel.Range)excel_sayfasi.Cells[i, 2];
                    hataEldeYazdir.Value2 = listboxHataElde.Items[i - 1];
                }
            }
        }


        private void btnCiktiExcel_Click(object sender, EventArgs e)
        {
            MExcel.Application excel_uygulamasi = new MExcel.Application();
            excel_uygulamasi.Visible = true;

            MExcel.Workbook excel_yazdir = excel_uygulamasi.Workbooks.Add(true);
            MExcel.Worksheet excel_sayfasi = (MExcel.Worksheet)excel_uygulamasi.Sheets[1];

            MExcel.Range ciktiYazdir = (MExcel.Range)excel_sayfasi.Cells[1, 1];
            ciktiYazdir.Value2 = noron6; //bunun için listbox gerekmez.

            MExcel.Range eldeYazdir = (MExcel.Range)excel_sayfasi.Cells[1, 2];
            eldeYazdir.Value2 = noron7; //bunun için listbox gerekmez.
        }

        private void listboxTemizle()
        {
            listboxHataCikti.Items.Clear();
            listboxHataElde.Items.Clear();
            listboxAgirlik1_4.Items.Clear();
            listboxAgirlik1_5.Items.Clear();
            listboxAgirlik2_4.Items.Clear();
            listboxAgirlik2_5.Items.Clear();
            listboxAgirlik3_4.Items.Clear();
            listboxAgirlik3_5.Items.Clear();
            listboxAgirlik4_6.Items.Clear();
            listboxAgirlik5_6.Items.Clear();
            listboxAgirlikE1_4.Items.Clear();
            listboxAgirlikE1_5.Items.Clear();
            listboxAgirlikE2_6.Items.Clear();
        }

    }
}
