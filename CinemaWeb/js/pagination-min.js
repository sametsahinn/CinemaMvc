 $(function(){
			//Total number of li
     var toplamLi = $("#paginationForm #paginationdiv").size();

			//The number of data page
                var veriSayisi = 6;

			//Implementing paging
                $("#paginationForm #paginationdiv:gt(" + (veriSayisi - 1) + ")").hide();

			//Let the number of pages
                var sayfaSayisi = Math.round(toplamLi / veriSayisi);

			//Add links to the page
              $("#pagination").append('<a href="javascript:void(0)" id="pagination-prev" data-num="prev"> < PREVİOUS</a>'); // sayıyı yuvarlayalım
                for (var i = 1; i <= sayfaSayisi; i++)
                {
                      $("#pagination").append('<a href="javascript:void(0)" id="pagination-a" data-num="'+i+'">' + i + '</a>'); // sayıyı yuvarlayalım
                }
              $("#pagination").append('<a href="javascript:void(0)" id="pagination-next" data-num="next">NEXT ></a>'); // sayıyı yuvarlayalım
				
			//Add first page of the active class
            $("#pagination a").eq(1).addClass("aktif");

			//Clicking on one of a paging in
            $("#pagination a").live("click", function(){
			   //receive the index value (one in the form of excess)
               var indis = $(this).attr("data-num");
               if(isNaN(indis)){
                   var aktifIndis = parseInt($("#pagination a.aktif").attr("data-num"));
                   if(indis == "prev" && aktifIndis > 1){
                        indis =  parseInt($("#pagination a.aktif").attr("data-num")) -1;
                   }else if(indis=="next" && aktifIndis < ($("#pagination a").length -2)){
                       indis =  parseInt($("#pagination a.aktif").attr("data-num")) +1;
                   }else{
                       return;
                   }
               }
			   //The total number of data appears to Find
               var gt = veriSayisi * indis;
                
			   //active class actions
               $("#pagination a").removeClass("aktif");

               //$(this).addClass("aktif");
                $('#pagination a[data-num="'+indis+'"]').addClass("aktif");
			   //Hide all lie in list
                $("#1a #paginationForm #paginationdiv").hide();

                //document.getElementById(paginationForm).style.display = 'none';
                
			   //for the total number of data appear with - do verisayi process and show up verisayi
               for (i = gt - veriSayisi; i < gt; i++)
               {
                   $("#1a #paginationForm #paginationdiv:eq(" + i + ")").show();
                   //document.getElementById(paginationForm).style.display = 'block';
               }
            });
        });
		
		$(function(){
        
			data = 5;
			var l = 0;
			$(".widget-main-default ul li:gt("+(data-1)+")").hide();
			$(".button").click(function(){
			   var time = 100;
				$('.widget-main-default ul li:hidden').each(function(){
                time = time + 150;
                $(this).delay(time).slideDown(500);
            });
			if(l==0){
				
			 $(".widget-main-default ul li:gt("+(data-1)+")");
			 l++;
			 $(this).text("Gizle")
			}

			else{
				$(".widget-main-default ul li:gt("+(data-1)+")").slideUp(900);;
				$(this).text("Tümünü Göster")
				l=0;
			}
				
			});
	});