using DummyAdminPanel.Repositories;
using Microsoft.AspNetCore.Components;

namespace DummyAdminPanel.Shared;

public partial class TableView<TData>
{
    [Parameter]
    public RenderFragment Header { get; set; } = null!;
    
    [Parameter]
    public RenderFragment<TData> RowTemplate { get; set; } = null!;

    [Parameter]
    public IEnumerable<TData> Data { get; set; } = null!;
}