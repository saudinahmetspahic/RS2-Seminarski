﻿using eKino.Model;
using eKino.Model.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Windows;
using System.Windows.Forms;

namespace eKino.Desktop
{
    public partial class frmAccountUredi : Form
    {
        private Korisnik _korisnik = null;
        private readonly ApiService _korisnikService = new ApiService("Korisnik");
        private readonly ApiService _gradService = new ApiService("Grad");
        private readonly ApiService _ulogaService = new ApiService("Uloga");
        public frmAccountUredi()
        {
            InitializeComponent();
            this.AutoValidate = AutoValidate.Disable;
            _korisnik = _korisnikService.Get<List<Korisnik>>(new KorisnikSearchRequest { Email = ApiService.Email }).FirstOrDefault();

        }

        private void frmAccountUredi_Load(object sender, EventArgs e)
        {

            txtIme.Text = _korisnik.Ime;
            txtPrezime.Text = _korisnik.Prezime;
            txtEmail.Text = _korisnik.Email;
            var gradLista = _gradService.Get<List<Grad>>();
            gradLista.Insert(0, new Grad { Id = 0, Naziv = "Odaberi grad" });
            cbxGrad.DataSource = gradLista;
            cbxGrad.DisplayMember = "Naziv";
            cbxGrad.ValueMember = "Id";
            var grad = gradLista.Where(w => w.Id == _korisnik.GradId).Select(s => s.Naziv).FirstOrDefault();
            var index = cbxGrad.FindString(grad);
            cbxGrad.SelectedIndex = index;
            dtpDatumRodjenja.Value = _korisnik.DatumRodjenja;

        }

        private void bttnSnimi_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                if (txtLozinka.Text != txtLozinkaPotvrda.Text)
                {
                    var r = System.Windows.Forms.MessageBox.Show("Greska: Lozinke se ne poklapaju.", "Lozinka", MessageBoxButtons.OK);
                    if (r.Equals(MessageBoxResult.OK))
                        return;
                }
                _korisnikService.Update<Model.Korisnik>(_korisnik.Id, new KorisnikInsertRequest
                {
                    Ime = txtIme.Text,
                    Prezime = txtPrezime.Text,
                    Email = txtEmail.Text,
                    GradId = cbxGrad.SelectedIndex,
                    DatumRodjenja = dtpDatumRodjenja.Value,
                    DatumRegistracije = _korisnik.DatumRegistracije,
                    Password = txtLozinka.Text,
                    PasswordPotvrda = txtLozinkaPotvrda.Text
                });
                if (!string.IsNullOrEmpty(txtLozinka.Text))
                {
                    frmLogin frmL = new frmLogin();
                    frmL.Show();
                    this.MdiParent.Close();
                    this.Close();
                }
                else
                {
                    frmPocetna frmP = new frmPocetna();
                    frmP.Show();
                    this.MdiParent.Close();
                    this.Close();
                }
            }
        }

        private void txtIme_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIme.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(txtIme, Messages.Text_Ime);
            }
        }

        private void txtPrezime_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPrezime.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(txtPrezime, Messages.Text_Prezime);
            }
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                e.Cancel = true;
                errorProvider.SetError(txtEmail, Messages.Text_Email);
            }
            else
            {
                try
                {
                    MailAddress mail = new MailAddress(txtEmail.Text);
                }
                catch (Exception)
                {
                    e.Cancel = true;
                    errorProvider.SetError(txtEmail, Messages.Text_Email);
                }
            }
        }

        private void txtLozinka_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLozinka.Text) && (txtLozinka.Text.Length < 6 || txtLozinka.Text.Length > 15))
            {
                e.Cancel = true;
                errorProvider.SetError(txtLozinka, Messages.Text_Sifra);
            }
        }

        private void txtLozinkaPotvrda_Validating(object sender, CancelEventArgs e)
        {
            if (txtLozinka.Text != txtLozinkaPotvrda.Text)
            {
                e.Cancel = true;
                errorProvider.SetError(txtLozinkaPotvrda, Messages.Text_Sifra_Potvrda);
            }
        }

        private void cbxGrad_Validating(object sender, CancelEventArgs e)
        {
            if (cbxGrad.SelectedIndex == 0)
            {
                e.Cancel = true;
                errorProvider.SetError(txtLozinka, Messages.Text_Sifra);
            }
        }

    }
}
