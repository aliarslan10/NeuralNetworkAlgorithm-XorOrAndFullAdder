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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            agiKur();
            btnTestEt.Enabled = false;
        }

        float ogrenmeKatsayisi = 0.9f;
        float momentum = 0.8f; //önerilen: 0.6 ile 0.8 arası.
        int iterasyonSayisi = 1000;

        float agirlik1_4, agirlik1_5;
        float agirlik2_4, agirlik2_5;
        float agirlik3_4, agirlik3_5;
        float agirlik4_6, agirlik5_6;
        float agirlikE1_4, agirlikE1_5, agirlikE2_6;

        float degisim1_4, degisim1_5;
        float degisim2_4, degisim2_5;
        float degisim3_4, degisim3_5;
        float degisim4_6, degisim5_6;
        float degisimE1_4, degisimE1_5, degisimE2_6;

        float noron1, noron2, noron3, noron4, noron5, noron6;
        float esik1 = 1.0f, esik2 = 1.0f;

        ListBox listboxHata = new ListBox();
        ListBox listboxAgirlik1_4 = new ListBox();
        ListBox listboxAgirlik1_5 = new ListBox();
        ListBox listboxAgirlik2_4 = new ListBox();
        ListBox listboxAgirlik2_5 = new ListBox();
        ListBox listboxAgirlik3_4 = new ListBox();
        ListBox listboxAgirlik3_5 = new ListBox();
        ListBox listboxAgirlik4_6 = new ListBox();
        ListBox listboxAgirlik5_6 = new ListBox();
        ListBox listboxAgirlikE1_4 = new ListBox();
        ListBox listboxAgirlikE1_5 = new ListBox();
        ListBox listboxAgirlikE2_6 = new ListBox();

        Random uret = new Random(); //her defasında farklı değerler üretmesi için public olması gerekiyor.
        private float rastgeleAgirlikUret()
        {
            return (float)uret.Next(1,999) / 1000.0f;
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

            degisim1_4 = 0; degisim1_5 = 0;
            degisim2_4 = 0; degisim2_5 = 0;
            degisim3_4 = 0; degisim3_5 = 0;
            degisim4_6 = 0; degisim5_6 = 0;
            degisimE1_4 = 0; degisimE1_5 = 0;
            degisimE2_6 = 0;

            noron1 = 0.0f;  noron2 = 0.0f; noron3 = 0.0f;
            noron4 = 0.0f;  noron5 = 0.0f; noron6 = 0.0f;

            lblN4Cikis.Text = "0.000000";
            lblN5Cikis.Text = "0.000000";
            lblN6Cikis.Text = "0.000000";

            butonlariAktifEt();
            excelButonlariPasif();
            listboxTemizle();
        }

        private float aktivasyon(float x) //sigmoid fonksiyonu
        {
            return (float)(1.0f / (1.0f + Math.Exp(-x)));
        }

        private float ileriHesaplamaIslemleri(float giris1, float giris2)
        {
            noron1 = (float)giris1;
            noron2 = (float)giris2;

            noron4 = (noron1 * agirlik1_4) + (noron2 * agirlik2_4) + (esik1 * agirlikE1_4);
            noron4 = aktivasyon(noron4);

            noron5 = (noron1 * agirlik1_5) + (noron2 * agirlik2_5) + (esik1 * agirlikE1_5);
            noron5 = aktivasyon(noron5);

            noron6 = (noron4 * agirlik4_6) + (noron5 * agirlik5_6) + (esik2 * agirlikE2_6);
            noron6 = aktivasyon(noron6);
           

            lblN4Cikis.Text = noron4.ToString();
            lblN5Cikis.Text = noron5.ToString();
            lblN6Cikis.Text = noron6.ToString();

            lblGiris1.Text = noron1.ToString();
            lblGiris2.Text = noron2.ToString();
            lblGiris3.Text = noron3.ToString();

            return noron6;
        }

 
        private void geriHesaplamaIslemleri(float giris1, float giris2, float istenenCikis) //backpropagation ile eğitim
        {
            ileriHesaplamaIslemleri(giris1, giris2);
            Application.DoEvents();

            //hata hesapları:
            float hataN6 = (noron6) * (1 - noron6) * (istenenCikis - noron6); //çıkıştaki hata hesabı
            float hataN5 = noron5 * (1 - noron5) * (hataN6 * agirlik5_6); // ara katmandaki hata hesabı
            float hataN4 = noron4 * (1 - noron4) * (hataN6 * agirlik4_6);//ara katmandaki hata hesabı

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

        }


        private float ileriHesaplamaIslemleri3girisli(float giris1, float giris2, float giris3)
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


            lblN4Cikis.Text = noron4.ToString();
            lblN5Cikis.Text = noron5.ToString();
            lblN6Cikis.Text = noron6.ToString();

            lblGiris1.Text = noron1.ToString();
            lblGiris2.Text = noron2.ToString();
            lblGiris3.Text = noron3.ToString();


            listboxAgirlik1_4.Items.Add(agirlik1_4); listboxAgirlik1_5.Items.Add(agirlik1_5);
            listboxAgirlik2_4.Items.Add(agirlik2_4); listboxAgirlik2_5.Items.Add(agirlik2_5);
            listboxAgirlik3_4.Items.Add(agirlik3_4); listboxAgirlik3_5.Items.Add(agirlik3_5);
            listboxAgirlik4_6.Items.Add(agirlik4_6); listboxAgirlik5_6.Items.Add(agirlik5_6);
            listboxAgirlikE1_4.Items.Add(agirlikE1_4); listboxAgirlikE1_5.Items.Add(agirlikE1_5);
            listboxAgirlikE2_6.Items.Add(agirlikE2_6);

            return noron6;
        }

        private void geriHesaplamaIslemleri3girisli(float giris1, float giris2, float giris3, float istenenCikis) //backpropagation ile eğitim
        {
            ileriHesaplamaIslemleri3girisli(giris1, giris2, giris3);
            Application.DoEvents();

            //hata hesapları:
            float hataN6 = (noron6) * (1 - noron6) * (istenenCikis - noron6); //çıkıştaki hata hesabı
            float hataN5 = noron5 * (1 - noron5) * (hataN6 * agirlik5_6); // ara katmandaki hata hesabı
            float hataN4 = noron4 * (1 - noron4) * (hataN6 * agirlik4_6);//ara katmandaki hata hesabı

            //Listbox Doldur Hata:
            listboxHata.Items.Add(Math.Abs(hataN6));

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

        }

        private void btnTestEt_Click(object sender, EventArgs e)
        {
            try
            { 
                noron1 = float.Parse(tbGiris1.Text);
                noron2 = float.Parse(tbGiris2.Text);
            }

            catch
            {
                MessageBox.Show("Boş giriş (kutucuk) bırakmayın", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            lblN6Cikis.Text = ileriHesaplamaIslemleri(noron1, noron2).ToString();
 
        }

        
        private void btnXorEgit_Click(object sender, EventArgs e)
        {
            excelButonlariPasif();
            butonlariPasifEt();
            for (int i = 0; i <= iterasyonSayisi; i++)
            {
                geriHesaplamaIslemleri(0, 0, 0);
                geriHesaplamaIslemleri(0, 1, 1);
                geriHesaplamaIslemleri(1, 0, 1);
                geriHesaplamaIslemleri(1, 1, 0);
            }

            if(noron6 > 0.45) //işlem sonundaki çıkış noronu sıfıra yaklaşmamışsa, 5000 ile eğitmeye devam et.
            {
                for (int i = 0; i <= 20000; i++)
                {
                    geriHesaplamaIslemleri(0, 0, 0);
                    geriHesaplamaIslemleri(0, 1, 1);
                    geriHesaplamaIslemleri(1, 0, 1);
                    geriHesaplamaIslemleri(1, 1, 0);
                }

                if(noron6 > 0.30)
                {
                    MessageBox.Show("Toplu XOR eğitiminde çok az da olsa bazen böyle istenmeyen sonuçlar çıkabiliyor.\nAğı yeniden kurup tekrardan XOR'a göre eğitmeyi deneyin.");
                }
            }

            lblEgitimAciklamasi.Text = "İki girişli XOR için ağ eğitildi.\nAşağıdaki kutucuklara giriş\nvererek XOR için eğitilen ağın\nsonuçlarını kontrol edebilirsiniz.";
            btnTestEt.Enabled = true;
            btnAgKur.Enabled = true;
        }

        private void btnAndEgit_Click(object sender, EventArgs e)
        {
            excelButonlariPasif();
            butonlariPasifEt();
            for (int i = 0; i <= iterasyonSayisi; i++)
            {
                geriHesaplamaIslemleri(0, 0, 0);
                geriHesaplamaIslemleri(0, 1, 0);
                geriHesaplamaIslemleri(1, 0, 0);
                geriHesaplamaIslemleri(1, 1, 1);
                // Application.DoEvents();
            }

            lblEgitimAciklamasi.Text = "İki girişli AND için ağ eğitildi.\nAşağıdaki kutucuklara giriş\nvererek AND için eğitilen ağın\nsonuçlarını kontrol edebilirsiniz.";
            btnTestEt.Enabled = true;
            btnAgKur.Enabled = true;
        }


        private void btnOrEgit_Click(object sender, EventArgs e)
        {
            excelButonlariPasif();
            butonlariPasifEt();
            for (int i = 0; i <= iterasyonSayisi; i++)
            {
                geriHesaplamaIslemleri(0, 0, 0);
                geriHesaplamaIslemleri(0, 1, 1);
                geriHesaplamaIslemleri(1, 0, 1);
                geriHesaplamaIslemleri(1, 1, 1);
                // Application.DoEvents();
            }

            lblEgitimAciklamasi.Text = "İki girişli OR için ağ eğitildi.\nAşağıdaki kutucuklara giriş\nvererek OR için eğitilen ağın\nsonuçlarını kontrol edebilirsiniz.";
            btnTestEt.Enabled = true;
            btnAgKur.Enabled = true;
        }

        private void btnXorEgit3Giris_Click(object sender, EventArgs e)
        {
            butonlariPasifEt();
            excelButonlariPasif();
            float giris1, giris2, giris3, cikis=0.0f;
            try
            {
                giris1 = float.Parse(tb3Girisli1.Text);
                giris2 = float.Parse(tb3Girisli2.Text);
                giris3 = float.Parse(tb3Girisli3.Text);


                if (giris1 == 0 && giris2 == 0 && giris3 == 0)
                    cikis = 0;
                if (giris1 == 0 && giris2 == 0 && giris3 == 1)
                    cikis = 1;
                if (giris1 == 0 && giris2 == 1 && giris3 == 0)
                    cikis = 1;
                if (giris1 == 0 && giris2 == 1 && giris3 == 1)
                    cikis = 0;
                if (giris1 == 1 && giris2 == 0 && giris3 == 0)
                    cikis = 1;
                if (giris1 == 1 && giris2 == 0 && giris3 == 1)
                    cikis = 0;
                if (giris1 == 1 && giris2 == 1 && giris3 == 0)
                    cikis = 0;
                if (giris1 == 1 && giris2 == 1 && giris3 == 1)
                    cikis = 1;

                for (int i = 0; i <= iterasyonSayisi; i++)
                {
                    geriHesaplamaIslemleri3girisli(giris1, giris2, giris3, cikis);
                }

                excelButonlariAktif();
            }

            catch
            {
                MessageBox.Show("Boş giriş (kutucuk) bırakmayın", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        private void btnOrEgit3Giris_Click(object sender, EventArgs e)
        {
            butonlariPasifEt();
            excelButonlariPasif();
            float giris1, giris2, giris3, cikis = 0.0f;
            try
            {
                giris1 = float.Parse(tb3Girisli1.Text);
                giris2 = float.Parse(tb3Girisli2.Text);
                giris3 = float.Parse(tb3Girisli3.Text);


                if (giris1 == 0 && giris2 == 0 && giris3 == 0)
                    cikis = 0;
                if (giris1 == 0 && giris2 == 0 && giris3 == 1)
                    cikis = 1;
                if (giris1 == 0 && giris2 == 1 && giris3 == 0)
                    cikis = 1;
                if (giris1 == 0 && giris2 == 1 && giris3 == 1)
                    cikis = 1;
                if (giris1 == 1 && giris2 == 0 && giris3 == 0)
                    cikis = 1;
                if (giris1 == 1 && giris2 == 0 && giris3 == 1)
                    cikis = 1;
                if (giris1 == 1 && giris2 == 1 && giris3 == 0)
                    cikis = 1;
                if (giris1 == 1 && giris2 == 1 && giris3 == 1)
                    cikis = 1;

                for (int i = 0; i <= iterasyonSayisi; i++)
                {
                    geriHesaplamaIslemleri3girisli(giris1, giris2, giris3, cikis);
                }
            }

            catch
            {
                MessageBox.Show("Boş giriş (kutucuk) bırakmayın", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            excelButonlariAktif();
        }

        private void btnAndEgit3Giris_Click(object sender, EventArgs e)
        {
            butonlariPasifEt();
            excelButonlariPasif();
            try
            {
                float giris1, giris2, giris3, cikis = 0.0f;
                giris1 = float.Parse(tb3Girisli1.Text);
                giris2 = float.Parse(tb3Girisli2.Text);
                giris3 = float.Parse(tb3Girisli3.Text);


                if (giris1 == 0 && giris2 == 0 && giris3 == 0)
                    cikis = 0;
                if (giris1 == 0 && giris2 == 0 && giris3 == 1)
                    cikis = 0;
                if (giris1 == 0 && giris2 == 1 && giris3 == 0)
                    cikis = 0;
                if (giris1 == 0 && giris2 == 1 && giris3 == 1)
                    cikis = 0;
                if (giris1 == 1 && giris2 == 0 && giris3 == 0)
                    cikis = 0;
                if (giris1 == 1 && giris2 == 0 && giris3 == 1)
                    cikis = 0;
                if (giris1 == 1 && giris2 == 1 && giris3 == 0)
                    cikis = 0;
                if (giris1 == 1 && giris2 == 1 && giris3 == 1)
                    cikis = 1;

                for (int i = 0; i <= iterasyonSayisi; i++)
                {
                    geriHesaplamaIslemleri3girisli(giris1, giris2, giris3, cikis);
                }

                excelButonlariAktif();
            }

            catch
            {
                MessageBox.Show("Boş giriş (kutucuk) bırakmayın", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAgKur_Click(object sender, EventArgs e)
        {
            agiKur();

            tbGiris1.Text = "";
            tbGiris2.Text = "";

            tb3Girisli1.Text = "";
            tb3Girisli2.Text = "";
            tb3Girisli3.Text = "";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
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

            grafik.DrawEllipse(kalem1, 450, 120, 75, 75);
            grafik.DrawString("N6", new Font("Arial", 10, FontStyle.Regular), firca, 480, 140);
            

            //Kalem çizimleri
            grafik.DrawLine(kalem3, 95, 50, 270, 90);    //n1-n4
            grafik.DrawLine(kalem3, 95, 50, 270, 210);   //n1-n5
            grafik.DrawLine(kalem3, 95, 150, 270, 90);   //n2-n4
            grafik.DrawLine(kalem3, 95, 150, 270, 210);  //n2-n5
            grafik.DrawLine(kalem3, 95, 250, 270, 90);   //n3-n4
            grafik.DrawLine(kalem3, 95, 250, 270, 210);  //n3-n5

            grafik.DrawLine(kalem3, 140, 300, 270, 90);  //e1-n4
            grafik.DrawLine(kalem3, 140, 300, 270, 210); //e1-n5

            grafik.DrawLine(kalem3, 345, 100, 450, 160);  //n4-n6
            grafik.DrawLine(kalem3, 345, 210, 450, 160);  //n5-n6
            grafik.DrawLine(kalem3, 350, 300, 450, 160);  //e2-n6
        }

        private void btnForm2_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
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

        private void btnBilgi_Click(object sender, EventArgs e)
        {
            MessageBox.Show("NOT : Bu kısımda ikili girişler ile test yapmadan önce üstteki\n'2 Girişli Olan Tüm Veriler İçin Ağı Eğit' kısmından XOR, OR yada AND'e göre ağı eğitin. Daha sonra 'eğitilen ağa' bu kısımdan girişler verip ağın öğrenip öğrenmediğini kontrol edebilirsiniz");
        }

        private void butonlariAktifEt()
        {
            btnAgKur.Enabled = false;
            btnXorEgit.Enabled = true;
            btnAndEgit.Enabled = true;
            btnOrEgit.Enabled = true;
            btnAndEgit.Enabled = true;
            btnAndEgit3Giris.Enabled = true;
            btnOrEgit3Giris.Enabled = true;
            btnXorEgit3Giris.Enabled = true;
            btnTestEt.Enabled = false;

            btnHataExcel.Enabled = true;
            btnCiktiExcel.Enabled = true;
        }

        private void butonlariPasifEt()
        {
            btnAgKur.Enabled = false;
            btnXorEgit.Enabled = false;
            btnAndEgit.Enabled = false;
            btnOrEgit.Enabled = false;
            btnAndEgit.Enabled = false;
            btnAndEgit3Giris.Enabled = false;
            btnOrEgit3Giris.Enabled = false;
            btnXorEgit3Giris.Enabled = false;
        }

        public void excelButonlariPasif()
        {
            btnHataExcel.Enabled = false;
            btnCiktiExcel.Enabled = false;
        }

        public void excelButonlariAktif()
        {
            btnHataExcel.Enabled = true;
            btnCiktiExcel.Enabled = true;
            btnAgKur.Enabled = true;
        }

        private void btnExcel_Click(object sender, EventArgs e) //hata değişimi fonksiyonu
        {
            MExcel.Application excel_uygulamasi = new MExcel.Application();
            excel_uygulamasi.Visible = true;

            MExcel.Workbook excel_yazdir = excel_uygulamasi.Workbooks.Add(true);
            MExcel.Worksheet excel_sayfasi = (MExcel.Worksheet)excel_uygulamasi.Sheets[1];


            int lbEleman_sayisi = listboxHata.Items.Count;
            for (int i = 1; i <= lbEleman_sayisi; i++)
            {
                MExcel.Range hataYazdir = (MExcel.Range)excel_sayfasi.Cells[i, 1];
                hataYazdir.Value2 = listboxHata.Items[i-1];
            }
        }


        private void btnCiktiExcel_Click(object sender, EventArgs e)
        {
            MExcel.Application excel_uygulamasi = new MExcel.Application();
            excel_uygulamasi.Visible = true;

            MExcel.Workbook excel_yazdir = excel_uygulamasi.Workbooks.Add(true);
            MExcel.Worksheet excel_sayfasi = (MExcel.Worksheet)excel_uygulamasi.Sheets[1];

            MExcel.Range ciktiYazdir = (MExcel.Range)excel_sayfasi.Cells[1, 1];
            ciktiYazdir.Value2 = noron6; //bunun için listbox gerekmiyor.
        }

        private void listboxTemizle()
        {
            listboxHata.Items.Clear();
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
