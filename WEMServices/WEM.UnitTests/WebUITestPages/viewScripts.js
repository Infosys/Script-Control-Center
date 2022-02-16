
var scriptDetailsVO = new Object();

jQuery182(document).ready(function($){

	jQuery182('#updateScript').hide();
	jQuery182('#repsonse').hide();

	//alert("On Load");
	var urlString = "https://scc-servername/iapwemservices/WEMCommonService.svc/GetAllCategoriesByCompany?companyId=1&module=2";

	jQuery182.ajax({
		type:"get",
		url :urlString,
		contentType: "application/json;charset=utf-8",
		cache : false,
		success : function(data) {
			populateCategories(data);
		},
		error : function(error, status) {
			alert("Error");
			alert(JSON.stringify(error));
		},
		async : false
	});	
});

function populateCategories(data) {

	jQuery182('#categoryName').empty();
	var seloption = '<option value="">Select Category</option>';

	for(var i=0;i<data.Categories.length;i++)
	{
		//alert(JSON.stringify(data.Categories[i]));
		seloption += '<option value="' + data.Categories[i].CategoryId + '">' + data.Categories[i].Name
		+ '</option>';
	}

	jQuery182('#categoryName').append(seloption);

}

function scriptDetails(){

	jQuery182('#updateScript').hide();
	var categoryId=jQuery182('#categoryName').val();
	//alert(categoryId);
	/*if(isNaN(categoryId)){
		break ;
	}*/
	var urlString = "https://scc-servername/iapwemservices/WEMScriptService.svc/GetAllScriptDetails/"+categoryId;

	jQuery182.ajax({
		type:"get",
		url :urlString,
		contentType: "application/json;charset=utf-8",
		cache : false,
		success : function(data, status) {
			populateScriptDetails(data);
		},
		error : function(error, status) {
			alert("Error");
			alert(JSON.stringify(error));
		},
		async : false
	});	

}

function populateScriptDetails(data) {

	var list = [];
	var scriptVo = new Object();

	for(var i=0;i<data.Scripts.length;i++)
	{
		scriptVo = new Object();
		scriptVo.Name = data.Scripts[i].Name ;
		scriptVo.Description = data.Scripts[i].Description ;
		scriptVo.URL = data.Scripts[i].ScriptURL ;
		scriptVo.ArgString = data.Scripts[i].ArgString ;
		scriptVo.BelongsToAccount = data.Scripts[i].BelongsToAccount ;
		scriptVo.BelongsToOrg = data.Scripts[i].BelongsToOrg ;
		scriptVo.BelongsToTrack = data.Scripts[i].BelongsToTrack ;
		scriptVo.CallMethod = data.Scripts[i].CallMethod ;
		scriptVo.CategoryId = data.Scripts[i].CategoryId ;
		scriptVo.CreatedBy = data.Scripts[i].CreatedBy ;
		scriptVo.CreatedOn = data.Scripts[i].CreatedOn ;
		scriptVo.IfeaScriptName = data.Scripts[i].IfeaScriptName ;
		scriptVo.IsDeleted = data.Scripts[i].IsDeleted ;
		scriptVo.ModifiedBy = data.Scripts[i].ModifiedBy ;
		scriptVo.ModifiedOn = data.Scripts[i].ModifiedOn ;
		scriptVo.Parameters = data.Scripts[i].Parameters ;
		scriptVo.RunAsAdmin = data.Scripts[i].RunAsAdmin ;
		scriptVo.ScriptContent = data.Scripts[i].ScriptContent ;
		scriptVo.ScriptFileVersion = data.Scripts[i].ScriptFileVersion ;
		scriptVo.ScriptId = data.Scripts[i].ScriptId ;
		scriptVo.ScriptType = data.Scripts[i].ScriptType ;
		scriptVo.StorageBaseUrl = data.Scripts[i].StorageBaseUrl ;
		scriptVo.TaskCmd = data.Scripts[i].TaskCmd ;
		scriptVo.TaskType = data.Scripts[i].TaskType ;
		scriptVo.UsesUIAutomation = data.Scripts[i].UsesUIAutomation ;
		scriptVo.WorkingDir = data.Scripts[i].WorkingDir ;

		list.push(scriptVo);
	}

	jQuery182("#scriptDetails").GridUnload();
	jQuery182("#scriptDetails")
	.jqGrid(
			{
				data : data.Scripts,
				datatype : 'local',
				width : 880,
				height : 350,
				colNames : [ 'ScriptId','Script Name', 'Script Description', 'Download' ],
				colModel : [
				            {
				            	name : 'ScriptId',
				            	index : 'ScriptId',
				            	width : 50,
				            	sortable : false
				            },
				            {
				            	name : 'Name',
				            	index : 'Name',
				            	width : 150,
				            	sortable : false
				            }, {
				            	name : 'Description',
				            	index : 'Description',
				            	width : 200,
				            	sortable : false
				            }, {
				            	name : 'ScriptURL',
				            	index : 'ScriptURL',
				            	width : 250,
				            	sortable : false,
				            	formatter: 'showlink',
				            	formatoptions: {
				            		baseLinkUrl: '#'

				            	}

				            } ],
				            recordpos : 'left',
				            viewrecords : true,
				            ignoreCase : true,
				            altRows : false,
				            scroll : true,
				            //scrollOffset : 0,
				            //hoverrows : false,
				            altClass : 'myAltRowClass',

				            onCellSelect : function(rowid, iCol, cellcontent, e) {

				            	if(iCol == 1){
				            		scriptName =jQuery182("#scriptDetails").jqGrid('getRowData',rowid).Name;
				            		id =jQuery182("#scriptDetails").jqGrid('getRowData',rowid).ScriptId;
				            		update(list,id);
				            	}

				            	else if(iCol == 3){
				            		url =jQuery182("#scriptDetails").jqGrid('getRowData',rowid).ScriptURL;
				            		window.location.href = url;
				            	}
				            },
				            loadComplete : function() {
				            	var myGrid =jQuery182("#scriptDetails");
				            	var ids = myGrid.getDataIDs();
				            	for (var i = 0, idCount = ids.length; i < idCount; i++) {
				            		jQuery182("#"+ids[i]+" a",myGrid[0]).click(function(e) {
				            			var hash=e.currentTarget.hash;// string like "#?id=0"
				            			if (hash.substring(0,5) === '#?id=') {
				            				var id = hash.substring(5,hash.length);
				            				var text = this.textContent || this.innerText;
				            				//alert("clicked the row with id='"+id+"'. Link contain '"+text+"'");
				            				window.location.href = text;
				            			}
				            			e.preventDefault();
				            		});
				            	}   
				            }


			});

}

function update(list,id) {

	jQuery182('#updateScript').show();
	var scriptVo = new Object();
	for(var i=0;i<list.length;i++)
	{
		if(list[i].ScriptId==id){
			scriptVo = list[i];
		}
	}

	scriptDetailsVO = new Object();
	scriptDetailsVO = scriptVo;

	jQuery182('#name').val(scriptDetailsVO.Name);
	jQuery182('#description').val(scriptDetailsVO.Description);

}

function updateScriptDetails(){

	var newName = jQuery182("#name").val()
	var newDescription = jQuery182("#description").val() ; 
	scriptDetailsVO.Name = newName ;
	scriptDetailsVO.Description = newDescription ;

	var xmlhttp=new XMLHttpRequest();
	xmlhttp.open("GET","templateNew.xml",false);
	xmlhttp.send();
	var xmlDoc=xmlhttp.responseXML; 
	var x=xmlDoc.getElementsByTagName("Name")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.Name;
	var x=xmlDoc.getElementsByTagName("Description")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.Description;
	var x=xmlDoc.getElementsByTagName("CategoryId")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.CategoryId;
	var x=xmlDoc.getElementsByTagName("ScriptId")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.ScriptId;
	var x=xmlDoc.getElementsByTagName("ScriptType")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.ScriptType;
	var x=xmlDoc.getElementsByTagName("ScriptURL")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.URL;
	var x=xmlDoc.getElementsByTagName("TaskType")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.TaskType;
	var x=xmlDoc.getElementsByTagName("UsesUIAutomation")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.UsesUIAutomation;
	var x=xmlDoc.getElementsByTagName("ModifiedBy")[0].childNodes[0];
	x.nodeValue="admin";
	/*var x=xmlDoc.getElementsByTagName("ArgString")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.ArgString;
	var x=xmlDoc.getElementsByTagName("WorkingDir")[0].childNodes[0];
	x.nodeValue=scriptDetailsVO.WorkingDir;
	var usrName = HttpContext.Current.User.Identity.Name;
	alert(usrName)*/

	var xmlString = (new XMLSerializer()).serializeToString(xmlDoc);

	var urlString = "https://scc-servername/iapwemservices/WEMScriptService.svc/UpdateScript";
	var method = 'PUT';
	//alert("Make ajax call : "+xmlString);

	jQuery182.ajax({
		url : urlString,
		data : xmlString,
		cache : false,
		type : method,
		dataType : 'xml',
		contentType :'application/xml',
		success : function(data, status) {
			alert("Script Updated Successfully");
			scriptDetailsAfterUpdate(scriptDetailsVO.CategoryId);
		},
		error : function(error, status) {
			alert("Script Updated Successfully");
			scriptDetailsAfterUpdate(scriptDetailsVO.CategoryId);
			//alert(JSON.stringify(error));
		},
		async : false
	});
}

function scriptDetailsAfterUpdate(categoryId){

	jQuery182('#updateScript').hide();

	var urlString = "https://scc-servername/iapwemservices/WEMScriptService.svc/GetAllScriptDetails/"+categoryId;

	jQuery182.ajax({
		type:"get",
		url :urlString,
		contentType: "application/json;charset=utf-8",
		cache : false,
		success : function(data, status) {
			populateScriptDetails(data);
		},
		error : function(error, status) {
			alert("Error");
			alert(JSON.stringify(error));
		},
		async : false
	});	

}


//To add a Script

function addScriptDetails(){

	var category = jQuery182("#categoryName").val() ;
	var name = jQuery182("#name").val() ;
	var description = jQuery182("#description").val() ;
	var scriptType = jQuery182("#scriptType").val() ;
	var scriptExtn = jQuery182("#scriptExtn").val() ;
	var myFile = document.getElementById("myFile") ;
	var file = myFile.files[0];

	var reader = new FileReader();
	reader.onload = function(){
		var dataURL = reader.result;
		//var output = document.getElementById('output');
		//output.src = dataURL;
		var byteArray = _arrayBufferToBase64(toUTF8Array(dataURL)) ;
		//alert("Byte Array : "+byteArray)

		//Remaining part of code
		var xmlhttp=new XMLHttpRequest();
		xmlhttp.open("GET","templateAdd.xml",false);
		xmlhttp.send();
		var xmlDoc=xmlhttp.responseXML; 

		var x=xmlDoc.getElementsByTagName("Name")[0].childNodes[0];
		x.nodeValue=name;
		var x=xmlDoc.getElementsByTagName("Description")[0].childNodes[0];
		x.nodeValue=description;
		var x=xmlDoc.getElementsByTagName("CategoryId")[0].childNodes[0];
		x.nodeValue=category;
		var x=xmlDoc.getElementsByTagName("ScriptId")[0].childNodes[0];
		x.nodeValue="0";
		var x=xmlDoc.getElementsByTagName("ScriptType")[0].childNodes[0];
		x.nodeValue=scriptExtn;
		var x=xmlDoc.getElementsByTagName("ScriptContent")[0].childNodes[0];
		x.nodeValue=byteArray;
		var x=xmlDoc.getElementsByTagName("TaskType")[0].childNodes[0];
		x.nodeValue=scriptType;

		var xmlString = (new XMLSerializer()).serializeToString(xmlDoc);

		var urlString = "https://scc-servername/iapwemservices/WEMScriptService.svc/AddScript" ;
		var method = 'POST';
		//alert("Make ajax call : "+xmlString);

		jQuery182.ajax({
			url : urlString,
			data : xmlString,
			cache : false,
			type : method,
			dataType : 'xml',
			contentType :'application/xml',
			success : function(data, status) {
				alert("Script Added Successfully");
				window.document.location.href = 'ViewScripts.html';
				//scriptDetailsAfterUpdate(scriptDetailsVO.CategoryId);
			},
			error : function(error, status) {
				alert("Script Added Successfully");
				window.document.location.href = 'ViewScripts.html';
				//alert("ERROR Occurred");
				//alert(JSON.stringify(error));
			},
			async : false
		});
	};
	reader.readAsBinaryString(file);

}

function toUTF8Array(str) {
	var utf8 = [];
	for (var i=0; i < str.length; i++) {
		var charcode = str.charCodeAt(i);
		if (charcode < 0x80) utf8.push(charcode);
		else if (charcode < 0x800) {
			utf8.push(0xc0 | (charcode >> 6), 
					0x80 | (charcode & 0x3f));
		}
		else if (charcode < 0xd800 || charcode >= 0xe000) {
			utf8.push(0xe0 | (charcode >> 12), 
					0x80 | ((charcode>>6) & 0x3f), 
					0x80 | (charcode & 0x3f));
		}
		// surrogate pair
		else {
			i++;
			// UTF-16 encodes 0x10000-0x10FFFF by
			// subtracting 0x10000 and splitting the
			// 20 bits of 0x0-0xFFFFF into two halves
			charcode = 0x10000 + (((charcode & 0x3ff)<<10)
					| (str.charCodeAt(i) & 0x3ff));
			utf8.push(0xf0 | (charcode >>18), 
					0x80 | ((charcode>>12) & 0x3f), 
					0x80 | ((charcode>>6) & 0x3f), 
					0x80 | (charcode & 0x3f));
		}
	}
	return utf8;
}
function _arrayBufferToBase64( buffer ) {
	var binary = '';
	var bytes = new Uint8Array( buffer );
	var len = bytes.byteLength;
	for (var i = 0; i < len; i++) {
		binary += String.fromCharCode( bytes[ i ] );
	}
	return window.btoa( binary );
}


function executeScript(){

	var serverName = jQuery182("#serverName").val() ;
	var categoryId = jQuery182("#categoryId").val() ;
	var scriptId = jQuery182("#scriptId").val() ;

	var xmlhttp=new XMLHttpRequest();
	xmlhttp.open("GET","executeTemplate.xml",false);
	xmlhttp.send();
	var xmlDoc=xmlhttp.responseXML; 

	var x=xmlDoc.getElementsByTagName("CategoryId")[0].childNodes[0];
	x.nodeValue=categoryId;
	var x=xmlDoc.getElementsByTagName("ScriptId")[0].childNodes[0];
	x.nodeValue=scriptId;

	var xmlString = (new XMLSerializer()).serializeToString(xmlDoc);

	var urlString = serverName+":9001/iap/rest/ExecuteScript" ;
	var method = 'POST';

	alert("Execute : "+xmlString);
	alert("URL : "+urlString)

	jQuery182.ajax({
		url : urlString,
		data : xmlString,
		cache : false,
		type : 'POST',
		dataType : 'xml',
		contentType :'text/xml;charset=utf-8',
		success : function(data, status) {
			alert("Success");
			populateResponse(data);
		},
		error : function(error, status) {
			alert("ERROR Occurred");
			alert(JSON.stringify(error));
		},
		async : false
	});

}

function populateResponse(data) {

	jQuery182('#repsonse').show();

	jQuery182('#errorMessage').val(data.ErrorMessage);
	jQuery182('#inputCommand').val(data.InputCommand);
	jQuery182('#isSuccess').val(data.IsSuccess);
	jQuery182('#output').val(data.Output);

}

function getBase64Image(){    
	var p;var canvas = document.createElement("canvas");
	var img1=document.createElement("img");  
	p=document.getElementById("myFile").value;
	img1.setAttribute('src', p); 
	canvas.width = img1.width; 
	canvas.height = img1.height; 
	var ctx = canvas.getContext("2d"); 
	ctx.drawImage(img1, 0, 0); 
	var dataURL = canvas.toDataURL("image/png");
	alert(dataURL);
}

