// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
window.alert = function (name) {
    var iframe = document.createElement("IFRAME");
    iframe.style.display = "none";
    iframe.setAttribute("src", 'data:text/plain,');
    document.documentElement.appendChild(iframe);
    window.frames[0].window.alert(name);
    iframe.parentNode.removeChild(iframe);
}



$(function () {
    console.log("sss");
});

function isValidIP(ip) {
    var reg = /^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$/
    return reg.test(ip);
}

//jQuery事件触发
$('#tgstb').on('click-row.bs.table', function (e, row, $element) {
 
    //alert(JSON.stringify(row));
    //对IP地址进行校验
    if (!isValidIP(row.ip)) {
        alert("ip invalid! " + row.ip);
        return;
    }

    $.ajax({
        url: "/api/TgsInfo/GetNextIP?ip=" + row.ip,
        async: false,
        success: function (res) {
            //console.log(res);
            window.alert('以下IP未配置,建议按顺序使用以下IP\n'+res);
        }
    });
});