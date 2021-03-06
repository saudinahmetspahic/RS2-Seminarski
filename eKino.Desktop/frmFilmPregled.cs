﻿using eKino.Model;
using eKino.Model.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eKino.Desktop
{
    public partial class frmFilmPregled : Form
    {
        private readonly ApiService _filmService = new ApiService("Film");
        private readonly ApiService _zanrService = new ApiService("Zanr");
        private readonly ApiService _tipService = new ApiService("Tip");
        private readonly ApiService _ocijenaService = new ApiService("Ocijena");

        private List<Film> _filmovi;
        private List<Zanr> _zanrovi;
        private List<Tip> _tipovi;
        private int _stranica;

        public frmFilmPregled()
        {
            InitializeComponent();
            _zanrovi = _zanrService.Get<List<Zanr>>();
            _tipovi = _tipService.Get<List<Tip>>();
            _stranica = 1;
        }

        private void DodajFilm(Film film)
        {
            var tbl = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 1
            };

            PictureBox img;
            if (!(film.Slika == null || film.Slika.Length == 0))
                img = new PictureBox { Image = ImageConvertor.ConvertByteArrayToImage(film.Slika) };
            else
                img = new PictureBox();
            tbl.Controls.Add(img);
            img.Anchor = AnchorStyles.Top;
            img.Anchor = AnchorStyles.Bottom;
            img.Width = 100;
            img.MaximumSize = new Size(50, 50);
            img.SizeMode = PictureBoxSizeMode.Zoom;
            img.Cursor = Cursors.Arrow;

            var tbl_inside = new TableLayoutPanel
            {
                RowCount = 3,
                ColumnCount = 2
            };
            tbl_inside.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tbl_inside.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            tbl_inside.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            tbl_inside.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            tbl_inside.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            tbl_inside.Height = 40;

            var l1 = new Label { Text = "Naziv" };
            var l2 = new Label { Text = film.Naziv };
            var l3 = new Label { Text = "Zanr" };
            var l4 = new Label { Text = _zanrovi.Where(w => w.Id == film.ZanrId).Select(s => s.NazivZanra).FirstOrDefault() };
            var l5 = new Label { Text = "Tip" };
            var l6 = new Label { Text = _tipovi.Where(w => w.Id == film.TipId).Select(s => s.NazivTipa).FirstOrDefault() };
            tbl_inside.Controls.Add(l1);
            tbl_inside.Controls.Add(l2);
            tbl_inside.Controls.Add(l3);
            tbl_inside.Controls.Add(l4);
            tbl_inside.Controls.Add(l5);
            tbl_inside.Controls.Add(l6);

            l1.Dock = DockStyle.Fill;
            l2.Dock = DockStyle.Fill;
            l3.Dock = DockStyle.Fill;
            l4.Dock = DockStyle.Fill;
            l5.Dock = DockStyle.Fill;
            l6.Dock = DockStyle.Fill;
            l1.TextAlign = ContentAlignment.MiddleRight;
            l2.TextAlign = ContentAlignment.MiddleLeft;
            l3.TextAlign = ContentAlignment.MiddleRight;
            l4.TextAlign = ContentAlignment.MiddleLeft;
            l5.TextAlign = ContentAlignment.MiddleRight;
            l6.TextAlign = ContentAlignment.MiddleLeft;

            tbl.Controls.Add(tbl_inside);
            tbl_inside.Dock = DockStyle.Fill;
            tbl_inside.MaximumSize = new Size(0, 70);
            //tbl_inside.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single; // izbrisati

            tblListaFilmova.Controls.Add(tbl);
            tbl.Dock = DockStyle.Fill;
            img.Click += new EventHandler(PrikaziDetaljeFilma);
            img.Name = film.Id.ToString(); // cuva id filma zbog onclick eventa

            tblListaFilmova.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
        }

        private void PrikaziDetaljeFilma(object sender, EventArgs e)
        {
            var s = sender as PictureBox;
            var id = Int32.Parse(s.Name);
            var frm = new frmFilmDetalji(id);
            frm.MdiParent = this.MdiParent;
            frm.Show();
        }

        private void UcitajFilmove(List<Film> filmovi)
        {
            var count = (filmovi.Count > 8 ? 8 : filmovi.Count);
            if (filmovi.Count <= 8)
            {
                bttnBack.Visible = false;
                bttnNext.Visible = false;
            }
            else
            {
                bttnBack.Visible = true;
                bttnNext.Visible = true;
            }
            for (int i = 0; i < count; i++)
            {
                DodajFilm(filmovi[i]);
            }
        }

        private void frmFilmPregled_Load(object sender, EventArgs e)
        {
            _filmovi = _filmService.Get<List<Film>>();
            UcitajFilmove(_filmovi);

            var kategorije = _tipovi;
            kategorije.Insert(0, new Tip { Id = 0, NazivTipa = "Odaberi kategoriju" });
            cbxKategorijaFilter.DataSource = kategorije;
            cbxKategorijaFilter.DisplayMember = "NazivTipa";
            cbxKategorijaFilter.ValueMember = "Id";

            var zanrovi = _zanrovi;
            zanrovi.Insert(0, new Zanr { Id = 0, NazivZanra = "Odaberi zanr" });
            cbxZanrFilter.DataSource = zanrovi;
            cbxZanrFilter.DisplayMember = "NazivZanra";
            cbxZanrFilter.ValueMember = "Id";

            cbxPopularnoFilter.DataSource = new[] {
                new{ Id = 0, Opis = "Odaberi ocijenu"},
                new{ Id = 1, Opis = "★☆☆☆☆ (1)" },
                new{ Id = 2, Opis = "★★☆☆☆ (2)" },
                new{ Id = 3, Opis = "★★★☆☆ (3)" },
                new{ Id = 4, Opis = "★★★★☆ (4)" },
                new{ Id = 5, Opis = "★★★★★ (5)" }
            };
            cbxPopularnoFilter.ValueMember = "Id";
            cbxPopularnoFilter.DisplayMember = "Opis";

            //sistem preporuke

        }

        private void bttnBack_Click(object sender, EventArgs e)
        {
            if (_stranica == 1)
                return;
            tblListaFilmova.Controls.Clear();

            _stranica--;
            var count = (_filmovi.Count > _stranica * 8 ? _stranica * 8 : _filmovi.Count);
            for (int i = _stranica * 8 - 8; i < count; i++)
            {
                DodajFilm(_filmovi[i]);
            }
        }

        private void bttnNext_Click(object sender, EventArgs e)
        {
            if (_stranica * 8 > _filmovi.Count)
                return;
            tblListaFilmova.Controls.Clear();

            _stranica++;
            var count = (_filmovi.Count > _stranica * 8 ? _stranica * 8 : _filmovi.Count);
            for (int i = _stranica * 8 - 8; i < count; i++)
            {
                DodajFilm(_filmovi[i]);
            }
        }

        private void bttnFiltriraj_Click(object sender, EventArgs e)
        {
            var filmovi = _filmovi;
            if (!string.IsNullOrEmpty(txtNazivFilter.Text))
            {
                filmovi = filmovi.Where(w => w.Naziv.Contains(txtNazivFilter.Text)).ToList();
            }
            if ((int)cbxKategorijaFilter.SelectedValue > 0)
            {
                filmovi = filmovi.Where(w => w.TipId == (int)cbxKategorijaFilter.SelectedValue).ToList();
            }
            if ((int)cbxZanrFilter.SelectedValue > 0)
            {
                filmovi = filmovi.Where(w => w.ZanrId == (int)cbxZanrFilter.SelectedValue).ToList();
            }
            if ((int)cbxPopularnoFilter.SelectedValue > 0)
            {
                var ocijenjeniFilmovi = _ocijenaService.Get<List<Ocijena>>().GroupBy(g => g.FilmId);
                foreach (var f in ocijenjeniFilmovi)
                {
                    var ocijena = f.Average(a => a.DataOcijena);
                    if (ocijena != (int)cbxPopularnoFilter.SelectedValue)
                    {
                        filmovi = filmovi.Except(_filmovi.Where(w => w.Id == f.Select(s => s.FilmId).FirstOrDefault())).ToList();
                    }
                }

            }

            tblListaFilmova.Controls.Clear();
            UcitajFilmove(filmovi);
        }
    }
}
