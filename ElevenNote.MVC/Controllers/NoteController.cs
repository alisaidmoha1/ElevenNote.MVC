﻿using ElevenNote.Models;
using ElevenNote.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElevenNote.MVC.Controllers
{
    public class NoteController : Controller
    {
        [Authorize]
        // GET: Note
        public ActionResult Index()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new NoteService(userId);
            var model = service.GetNotes();
            return View(model);
        }

        // this is get a method. We are requesting to get Create View.
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create (NoteCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var service = CreateNoteService();

            if (service.CreateNote(model))
            {
                TempData["SaveResult"] = "You note was created.";
                return RedirectToAction("Index");
            };

            ModelState.AddModelError("", "Note could not be created.");

            return View(model);
        }

        public ActionResult Details (int id)
        {
            var svc = CreateNoteService();
            var model = svc.GetNoteById(id);

            return View(model);
        }

        public ActionResult Edit (int id)
        {
            var svc = CreateNoteService();
            var detail = svc.GetNoteById(id);
            var model =
                new NoteEdit
                {
                    NoteId = detail.NoteId,
                    Title = detail.Title,
                    Content = detail.Content
                };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit( int id, NoteEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if(model.NoteId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            var svc = CreateNoteService();

            if(svc.UpdateNote(model))
            {
                TempData["SaveResult"] = "You note was updated";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Your note could not be updated");
            return View(model);
        }

        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            var svc = CreateNoteService();
            var model = svc.GetNoteById(id);

            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var svc = CreateNoteService();
            svc.DeleteNote(id);

            TempData["SaveResult"] = "Your note was deleted";

            return RedirectToAction("Index");
        }

        private NoteService CreateNoteService()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var service = new NoteService(userId);
            return service;
        }
    }
}