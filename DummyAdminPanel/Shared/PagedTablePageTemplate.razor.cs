using DummyAdminPanel.Exceptions;
using DummyAdminPanel.Models;
using DummyAdminPanel.Repositories;
using Microsoft.AspNetCore.Components;
using System.Xml.Linq;

namespace DummyAdminPanel.Shared;

public partial class PagedTablePageTemplate<TData>
{
    [Parameter]
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage == value)
                return;

            _currentPage = value;
            _ = OnCurrentPageChanged(_isHasFirstRendered);
        }
    }
    int _currentPage;

    [Parameter]
    public Func<int, Task<PagedData<TData>?>> TryLoadDataForPage { get; set; } = null!;

    [Parameter]
    public Func<int, string> PageUrlGenerator { get; set; } = null!;

    [Parameter]
    public RenderFragment TableHeader { get; set; } = null!;

    [Parameter]
    public RenderFragment<TData> TableRowTemplate { get; set; } = null!;

    bool _isFailedToFetch;
    PagedData<TData>? _pagedData;

    // Due to pre-rendering in Blazor Server we can't perform any
    // JS interop until the OnAfterRender lifecycle method.
    // We need JS interop to access LocalStorage for caching purposes.
    bool _isHasFirstRendered;

    protected sealed override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ShowDataForCurrentPage();
            _isHasFirstRendered = true;
        }
    }

    async Task OnCurrentPageChanged(bool isHasFirstRendered)
    {
        if (isHasFirstRendered)
            await ShowDataForCurrentPage();
    }

    async Task ShowDataForCurrentPage()
    {
        var newData = await TryLoadDataForPage(CurrentPage);
        if (newData == null)
            _isFailedToFetch = true;
        _pagedData = newData;
        StateHasChanged();
    }
}