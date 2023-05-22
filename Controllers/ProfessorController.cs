﻿using Escola.Models;
using Escola.Models.ViewModels;
using Escola.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Escola.Controllers
{
    [Authorize(Roles = "Professor, Admin")]
    public class ProfessorController : Controller
    {
        private readonly IProfessorRepository _professorRepository;
        private readonly IDashboardRepository _dashboardRepo;
        private readonly IAlunoRepository _alunoRepo;
        private readonly INotaRepository _notaRepo;

        public ProfessorController(IProfessorRepository professorRepository, 
            IAlunoRepository alunoRepo,
            INotaRepository notaRepo,
            IDashboardRepository dashboardRepo)
        {
            _professorRepository = professorRepository;
            _alunoRepo = alunoRepo;
            _notaRepo = notaRepo;
            _dashboardRepo = dashboardRepo;
        }

        public IActionResult TurmasGeral()
        {
            var turmas = _professorRepository.GetTurmas(User.Identity.Name);
            return View(turmas);
        }

        [Route("Professor/Turma/{turmaId}")]
        public IActionResult Turma([FromRoute]int turmaId)
        {
            if (turmaId == 0)
                return NotFound();

            var usuariosNaTurma = _professorRepository.GetUsuariosNaTurma(turmaId);

            if (usuariosNaTurma == null)
                return NotFound();

            return View(usuariosNaTurma);
        }

        [HttpGet]
        [Route("Professor/AddNota/{alunoId}")]
        public IActionResult AddNota(string alunoId)
        {
            var vm = new AddNotaVM();
            var turma = _alunoRepo.GetTurmaById(alunoId);
            vm.AlunoFK = alunoId;
            vm.ProfessorFK = _dashboardRepo.GetUser(User.Identity.Name).Id;
            vm.TurmaFK = turma.Id;
            vm.MateriasProfessor = _professorRepository.GetMateriasProfessor(User.Identity.Name, turma.Id);

            return View(vm);
        }

        [HttpPost]
        [Route("Professor/AddNota/{alunoId}")]
        public IActionResult AddNota(AddNotaVM vm) 
        {
            if (ModelState.IsValid)
            {
                var nota = new Nota()
                {
                    AlunoFK = vm.AlunoFK,
                    BimestreFK = vm.BimestreFK,
                    ProfessorFK = vm.ProfessorFK,
                    Valor = vm.Nota,
                    Faltas = vm.Faltas,
                    MateriaFK = vm.MateriaFK,
                    TurmaFK = vm.TurmaFK,
                };


                _notaRepo.Add(nota);
                return RedirectToAction("Boletim", "Aluno", new { turmaId = vm.TurmaFK, alunoId = vm.AlunoFK });
            }

            return View(vm);
        }

        [HttpGet]
        [Route("Professor/ViewInfos/{alunoId}")]
        public IActionResult ViewInfos(string alunoId)
        {
            var vm = new NotasDoAlunoVM();

            vm = _notaRepo.GetNotasAddByProf(alunoId, User.Identity.Name);

            return View(vm);
        }

        [HttpGet]
        [Route("Professor/Edit/{notaId:int}")]
        public IActionResult Edit(int notaId)
        {
            var nota = _notaRepo.Get(notaId);

            var vm = new AddNotaVM()
            {
                AlunoFK = nota.AlunoFK,
                BimestreFK = nota.BimestreFK,
                Faltas = nota.Faltas,
                Nota = nota.Valor,
                MateriaFK = nota.MateriaFK,
                MateriasProfessor = _professorRepository.GetMateriasProfessor(User.Identity.Name, nota.TurmaFK),
                ProfessorFK = nota.ProfessorFK,
                NotaId = notaId,
                TurmaFK = nota.TurmaFK
            };


            return View(vm);
        }

        [HttpPost]
        [Route("Professor/Edit/{notaId:int}")]
        public IActionResult Edit(AddNotaVM vm)
        {
            if (ModelState.IsValid)
            {
                var nota = new Nota()
                {
                    Id = vm.NotaId,
                    AlunoFK = vm.AlunoFK,
                    BimestreFK = vm.BimestreFK,
                    ProfessorFK = vm.ProfessorFK,
                    Valor = vm.Nota,
                    Faltas = vm.Faltas,
                    MateriaFK = vm.MateriaFK,
                    TurmaFK = vm.TurmaFK,
                };

                _notaRepo.Update(nota);
                return RedirectToAction("Boletim", "Aluno", new { turmaId = vm.TurmaFK, alunoId = vm.AlunoFK });
            }

            return View(vm);
        }

        [HttpGet]
        [Route("Professor/Delete/{notaId:int}")]
        public IActionResult Delete(int notaId)
        {
            var vm = _notaRepo.Get(notaId);
            return View(vm);
        }

        [HttpPost]
        [Route("Professor/Delete/{notaId:int}")]
        public IActionResult DeletePost(int notaId)
        {
            _notaRepo.Delete(notaId);

            
            return RedirectToAction("TurmasGeral");
        }
    }
}
