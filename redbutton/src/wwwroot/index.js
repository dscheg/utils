$("#push").onclick = (e) => {
	fetchWithTimeout("api/push", {"method": "post", "credentials": "include"})
		.then(response => response.text().then(text => setResult(response.ok, text)))
		.catch(() => setResult(false, "api/push failed"));
};

function getCookie(name) {
	let val = document.cookie.split('; ').find(row => row.startsWith(name))?.split('=')[1];
	return val && decodeURIComponent(val);
}

const MAX_SIZE = 32;

function askParam(name) {
	do {
		var val = (getCookie(name) || prompt("Enter your " + name.toUpperCase())).substring(0, MAX_SIZE).trim();
	} while(!val || val.length === 0);
	document.cookie = name + "=" + encodeURIComponent(val) + "; samesite=strict" + (location.protocol === "https:" ? "; secure" : "");
	$("#" + name).textContent = val;
}

askParam("team");
