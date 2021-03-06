﻿using AutoMapper;
using eKino.API.Database;
using eKino.API.EF;
using eKino.Model;
using eKino.Model.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace eKino.API.Services
{
    public class KorisnikService
        :
        BaseCRUDService<Model.Korisnik, KorisnikSearchRequest, Database.Korisnik, KorisnikInsertRequest, KorisnikInsertRequest>
    {

        public KorisnikService(MojContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override Model.Korisnik Insert(KorisnikInsertRequest korisnik)
        {
            var k = _mapper.Map<Database.Korisnik>(korisnik);

            if (korisnik.Password != korisnik.PasswordPotvrda)
            {
                throw new Exception("Passwordi se ne slažu");
            }

            k.LozinkaSalt = GenerateSalt();
            k.LozinkaHash = GenerateHash(k.LozinkaSalt, korisnik.Password);

            _context.Korisnik.Add(k);
            _context.SaveChanges();
            return _mapper.Map<Model.Korisnik>(k);
        }

        public override Model.Korisnik Authenticate(KorisnikLoginRequest request)
        {
            var korisnik = _context.Korisnik.Include(i => i.Uloga).FirstOrDefault(x => x.Email == request.Email);

            if (korisnik != null)
            {
                var newHash = GenerateHash(korisnik.LozinkaSalt, request.Password);

                if (newHash == korisnik.LozinkaHash)
                {
                    Model.Korisnik user = _mapper.Map<Model.Korisnik>(korisnik);
                    return user;
                }
            }
            return null;
        }

        public static string GenerateSalt()
        {
            var buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        public static string GenerateHash(string salt, string password)
        {
            byte[] src = Convert.FromBase64String(salt);
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            byte[] dst = new byte[src.Length + bytes.Length];

            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);

            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }

        public override List<Model.Korisnik> Get(KorisnikSearchRequest request)
        {
            var query = _context.Korisnik.AsQueryable();

            if (request.Id > 0)
            {
                query = query.Where(w => w.Id == request.Id);
            }
            if (!string.IsNullOrEmpty(request?.Email))
            {
                query = query.Where(w => w.Email == request.Email);
            }
            if (!string.IsNullOrEmpty(request?.Ime))
            {
                query = query.Where(w => w.Ime == request.Ime);
            }
            if (!string.IsNullOrEmpty(request?.Prezime))
            {
                query = query.Where(w => w.Prezime == request.Prezime);
            }

            var result = query.ToList();
            return _mapper.Map<List<Model.Korisnik>>(result);
        }


        public override Model.Korisnik Update(int id, KorisnikInsertRequest korisnik)
        {
            var k = _context.Korisnik.SingleOrDefault(w => w.Id == id);
            if (k != null)
            {
                if (!string.IsNullOrEmpty(korisnik.Password))
                {
                    var salt = GenerateSalt();
                    var hash = GenerateHash(salt, korisnik.Password);
                    k.LozinkaSalt = salt;
                    k.LozinkaHash = hash;
                }
                k.Ime = korisnik.Ime;
                k.Prezime = korisnik.Prezime;
                k.Email = korisnik.Email;
                k.GradId = korisnik.GradId;
                k.DatumRodjenja = korisnik.DatumRodjenja;
                _context.Korisnik.Update(k);
                _context.SaveChanges();
            }
            return _mapper.Map<Model.Korisnik>(k);
        }

    }
}
