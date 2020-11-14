let update = () => fetchWithTimeout("api/result", {"method": "get", "credentials": "include"})
	.then(response => {
		if(!response.ok)
			response.text().then(text => setResult(false, text));
		else {
			response.json().then((json) => {
				$body = $(".results");
				while($body.lastElementChild){$body.removeChild($body.lastElementChild);}
				let template = $("#template").content;
				let $td = template.querySelectorAll("td");
				$td[0].textContent = "";
				$td[1].textContent = "round #" + (Number(json.rand) >>> 0).toString(16);
				$td[2].textContent = Math.floor(Number(json.time) / 10000000) + " sec";
				let clone = document.importNode(template, true);
				$body.appendChild(clone);
				for(var i = 0; i < json.list.length; i++) {
					$td[0].textContent = "#" + json.list[i].idx;
					$td[1].textContent = json.list[i].team;
					$td[2].textContent = (Math.floor(Number(json.list[i].time) / 10000) / 1000) + " sec";
					let clone = document.importNode(template, true);
					$body.appendChild(clone);
				}
			});
		}
	}).catch(() => setResult(false, "api/result failed"));

$$(".btn").forEach(item => item.onclick = (e) => {
	let action = e.currentTarget.id;
	fetchWithTimeout("api/" + action, {"method": "post", "credentials": "include"})
		.then(response => response.text().then(text => setResult(response.ok, text)))
		.catch(() => setResult(false, "api/" + action + " failed"));
});

setInterval(update, 1000);
