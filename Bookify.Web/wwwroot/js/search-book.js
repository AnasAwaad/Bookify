$(function () {


    var books = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/Search/FindBook?query=%QUERY',
            wildcard: '%QUERY'
        }
    });

    $('#SearchBook').typeahead({
        minLength: 4,
        highlight: true
    },
    {
        name: 'book',
        display: 'title',
        limit:100,
        source: books,
        templates: {
            empty: [
                '<div class="empty-message m-3 fs-4 text-primary">',
                'Unable to find the book you want',
                '</div>'
            ].join('\n'),
            suggestion: Handlebars.compile(`<div class="d-flex flex-stack">  
                                                <div class="symbol symbol-40px me-5">
                                                    <img src="{{imageThumbnailUrl}}" class="h-50 align-self-center" alt="{{title}}">                         
                                                </div>

                                                <div class="d-flex align-items-center flex-row-fluid flex-wrap">
                                                    <div class="flex-grow-1 me-2">
                                                        <h5 class="text-gray-800  fs-6 fw-bold">{{title}}</h5>
                        
                                                        <span class="text-muted fw-semibold d-block fs-7">by {{authorName}}</span>
                                                    </div>
                                                </div>
                                            </div > `)
        }
        }).on('typeahead:select', function (e,book) {
            window.location.replace(`/Search/Details?bookKey=${book.key}`)
        })
});