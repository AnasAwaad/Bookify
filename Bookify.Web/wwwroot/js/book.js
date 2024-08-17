$(function () {
	// On document ready


	$('#dataTableSearch').on('keyup', function () {
		var searchValue = $(this).val();
		DataTable.search(searchValue).draw();
	});

	var DataTable = $('#BookTbl').DataTable({
		serverSide: true,
		processing: true,
		stateSave: true,
		ajax: {
			url: "/Books/GetBooks",
			type: "POST"
		},
		order: [[1, 'asc']],
		columnDefs: [
			{
				targets: [0],
				searchable: false,
				visible: false
			}
		],
		columns: [
			{ "data": "id", "name": "Id", "className": "d-none" },
			{
				"name": "Title",
				"className": "d-flex align-items-center ",
				"render": function (data, type, row) {
					return `
									<div class="symbol symbol-50px overflow-hidden me-3">
										<a href="/Books/Details/${row.id}">
											<div class="symbol-label h-75">
												<img src="${(row.imageThumbnailUrl === null ? '/images/image-placeholder.jpg' : row.imageThumbnailUrl)}" alt="${row.title}" class="w-100">
											</div>
										</a>
									</div>

									<div class="d-flex flex-column">
										<a href="/Books/Details/${row.id}" class="text-primary mb-1">${row.title}</a>
										<span>${row.authorName}</span>
									</div>
									`;
				},
				"max-width": "200px"
			},
			{ "data": "publisher", "name": "Publisher" },
			{
				"name": "PublishingDate",
				"render": function (data, type, row) {
					return moment(row.publishingDate).format("ll");
				}
			},
			{ "data": "hall", "name": "Hall" },
			{ "data": "categories", "name": "Categories", "orderable": false },
			{
				"data": "isAvailableForRental",
				"name": "IsAvailableForRental",
				"render": function (data, type, row) {
					return `<span class=" badge badge-${row.isAvailableForRental ? "success" : "danger"}">
										${row.isAvailableForRental ? "Available" : "Not Available"}
									</span>`;
				}
			},
			{
				"name": "IsActive",
				"render": function (data, type, row) {
					return `<span class="js-toggle-status  badge badge-${row.isActive ? "success" : "danger"}">
										${row.isActive ? "Available" : "Not Available"}
									</span>`;
				}
			},
			{
				"orderable": false,
				"render": function (data, type, row) {
					return `
									<div class="dropdown">
										<button class="btn btn-sm btn-light btn-flex btn-center btn-active-light-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
											Actions
										</button>
										<ul class="dropdown-menu">
											<li><a class="dropdown-item" href="/Books/Update/${row.id}" >Update</a></li>
											<li><hr class="dropdown-divider"></li>
											<li><a class="dropdown-item js-btn-toggle-status" data-id="${row.id}" href="javaScript:;" data-url="/Books/ToggleStatus/${row.id}"">Toggle Status</a></li>
										</ul>
									</div>`;
				}
			}


		]
	});
	// Clear search input on page load
	$('#dataTableSearch').val('');
	DataTable.search('').draw();
})