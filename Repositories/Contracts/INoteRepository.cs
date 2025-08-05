using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface INoteRepository : IRepositoryBase<Note>
    {
        Task<IEnumerable<Note>> GetAllNotesAsync();
        Task<int> GetAllNotesCountAsync();
        Task<Note?> GetNoteByIdAsync(int noteId);
        void CreateNote(Note note);
        void DeleteNote(Note note);
        void UpdateNote(Note note);
    }
}
