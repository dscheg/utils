let $ = selector => document.querySelector(selector);
let $$ = selector => document.querySelectorAll(selector);

let $result = $("#result");
function setResult(ok, text) {
	$result.textContent = text || "ERROR";
	$result.classList.toggle(ok ? "ok" : "error", true);
	$result.classList.toggle(ok ? "error" : "ok", false);
}

function timeout(ms, promise, controller) {
	return new Promise((resolve, reject) => {
		let id = setTimeout(() => {
			id = undefined;
			if(controller) controller.abort();
			reject(new Error("timed out"));
		}, ms);
		promise.then(result => {
			if(id) {
				clearTimeout(id);
				resolve(result);
			}
		}, error => {
			if(id) {
				clearTimeout(id);
				reject(error);
			}
		});
	})
}

const FETCH_TIMEOUT = 800;

function fetchWithTimeout(url, options) {
	let controller = new AbortController();
	(options || (options = {})).signal = controller.signal;
	return timeout(FETCH_TIMEOUT, fetch(url, options), controller);
}
