﻿using ExamProgram.UI.ExamProgramUIExceptions;
using ExamProgram.UI.Services.Interfaces;
using ExamProgram.UI.ViewModels.ClassViewModels;
using ExamProgram.UI.ViewModels.StudentViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ExamProgram.UI.Controllers
{
    public class StudentController : Controller
    {
        private readonly ICrudService _crudService;

        public StudentController(ICrudService crudService)
        {
            _crudService = crudService;
        }
        public async Task<IActionResult> Index()
        {
            var datas = await _crudService.GetAllAsync<IEnumerable<StudentViewModel>>("/students/getall");

            return View(datas);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Classes = await _crudService.GetAllAsync<List<CLassViewModel>>("/class/getall");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            ViewBag.Classes = await _crudService.GetAllAsync<List<CLassViewModel>>("/class/getall");
            if(!ModelState.IsValid) return View(model);

            try
            {
                await _crudService.CreateAsync("/students/create", model);
            }
            catch(ApiException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    foreach (var key in ex.ModelErrors.Keys)
                    {
                        ModelState.AddModelError(key, ex.ModelErrors[key]);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Classes = await _crudService.GetAllAsync<List<CLassViewModel>>("/class/getall");
            var data = await _crudService.GetByIdAsync<StudentCreateViewModel>($"/students/get/{id}", id);
            if(data is null)
            {
                return NotFound();
            }
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, StudentCreateViewModel model)
        {
            ViewBag.Classes = await _crudService.GetAllAsync<List<CLassViewModel>>("/class/getall");
            try
            {
                await _crudService.UpdateAsync($"/students/update/{id}", model);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    foreach (var key in ex.ModelErrors.Keys)
                    {
                        ModelState.AddModelError(key, ex.ModelErrors[key]);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.DeleteAsync($"/students/delete/{id}", id);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    foreach (var key in ex.ModelErrors.Keys)
                    {
                        ModelState.AddModelError(key, ex.ModelErrors[key]);
                    }
                    return View();
                }else if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
