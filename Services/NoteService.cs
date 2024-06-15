using JadeMaui.Helpers;
using JadeMaui.Models;

namespace JadeMaui.Services;

public class NoteService : INoteService
{
    private readonly RestService _restService = ServiceHelper.GetService<RestService>();
    private string Route = "/Notes";

    public async Task<string?> GetNoteContent(string id) => await _restService.GetItem<string>($"{Route}/content/{id}");
    public async Task<Note?> GetNote(string id) => await _restService.GetItem<Note>($"{Route}/{id}");
    public async Task<List<Note>?> GetNotes() => await _restService.GetItems<Note>($"{Route}");
    public async Task<List<Note>?> GetArchiveNotes() => await _restService.GetItems<Note>($"{Route}/archive");
}
