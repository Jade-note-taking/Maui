using JadeMaui.Models;

namespace JadeMaui.Services;

public interface INoteService
{
    Task<string?> GetNoteContent(string id);
    Task<Note?> GetNote(string id);
    Task<List<Note>?> GetNotes();
}
