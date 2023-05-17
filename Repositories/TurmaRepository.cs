﻿using Escola.Data;
using Escola.Models;
using Escola.Models.ViewModels;
using Escola.Repositories.Contracts;

namespace Escola.Repositories
{
    public class TurmaRepository : RepositoryBase<Turma>, ITurmaRepository
    {
        private readonly ApplicationDbContext _db;

        public TurmaRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void AddUserTurma(AddUserTurmaVM vm)
        {
            var model = new TurmaUser();

            model.TurmaFK = vm.TurmaFK;
            model.UserFK = vm.UserFK;

            _db.TurmaUser.Add(model);
            _db.SaveChanges();
        }

        public TurmaUser GetTurmaUser(int turmaId, string userId)
        {
            var turmaUser = _db.TurmaUser.Where(x => x.UserFK == userId && x.TurmaFK == turmaId).FirstOrDefault();

            return turmaUser;
        }

        public void RemoverUserTurma(int turmaId, string userId)
        {
            var turmaUser = _db.TurmaUser.Where(x => x.UserFK == userId && x.TurmaFK == turmaId).FirstOrDefault();

            if (turmaUser != null) 
            {
                _db.TurmaUser.Remove(turmaUser);
                _db.SaveChanges();
            }
        }
    }
}
