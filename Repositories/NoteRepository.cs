using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class NoteRepository : RepositoryBase<Note>, INoteRepository
    {
        public NoteRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateNote(Note note)
        {
            Create(note);
        }

        public void DeleteNote(Note note)
        {
            Remove(note);
        }

        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            var notes = await FindAll(false)
                .ToListAsync();

            return notes;
        }

        public Task<int> GetAllNotesCountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Note?> GetNoteByIdAsync(int noteId)
        {
            var note = await FindByCondition(n => n.NoteId == noteId, true)
                .FirstOrDefaultAsync();

            return note;
        }

        public void UpdateNote(Note note)
        {
            Update(note);
        }
    }
}
