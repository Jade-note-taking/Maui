using JadeMaui.Models;

namespace JadeMaui.Services;

public interface INoteService
{
    Task<string?> GetNote(string id);
    Task<List<Note>?> GetNotes();
}
