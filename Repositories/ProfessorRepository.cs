﻿using Escola.Data;
using Escola.Models;
using Escola.Repositories.Contracts;
using System;

namespace Escola.Repositories
{
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly ApplicationDbContext _db;

        public ProfessorRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<ApplicationUser> GetAlunosNaTurma(int turmaId)
        {
            var turmaListadaComAlunos = _db.TurmaUser.Where(x => x.TurmaFK == turmaId).ToList();

            List<ApplicationUser> alunosNaTurma = new List<ApplicationUser>();

            foreach (var obj in turmaListadaComAlunos)
            {
                var aluno = _db.Users.Find(obj.UserFK);

                var isInRole = _db.UserRoles.FirstOrDefault(x => x.UserId == aluno.Id);

                if (isInRole.UserId == aluno.Id && isInRole.RoleId == "3")
                {
                    alunosNaTurma.Add(aluno);
                }
            }

            return alunosNaTurma;
        }

        public List<Turma> GetTurmas(string userName)
        {
            ApplicationUser professor = _db.Users.FirstOrDefault(x => x.UserName == userName);
            var turmasDoUser = _db.TurmaUser.Where(x => x.UserFK == professor.Id).ToList();

            List<Turma> turmas = new List<Turma>();

            foreach (var obj in turmasDoUser)
            {
                var turmaSelecionada = _db.Turmas.Find(obj.TurmaFK);

                turmas.Add(turmaSelecionada);
            };

            return turmas;
        }
    }
}
