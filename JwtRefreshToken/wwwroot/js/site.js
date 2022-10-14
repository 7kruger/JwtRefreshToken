let authorizated = false;
let token = { accessToken: "", refreshToken: "" };

if (authorizated) {
    loginFormVisible(false);
    authFormVisible(true);
} else {
    loginFormVisible(true);
    authFormVisible(false);
}

document.getElementById("loginBtn").onclick = () => {
    let login = document.getElementById("inputLogin").value;
    let pass = document.getElementById("inputPassword").value;

    fetch("https://localhost:7174/api/users/authenticate", {
        method: "post",
        headers: {
            "content-type": "application/json",
        },
        body: JSON.stringify({ name: login, password: pass })
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            }
            else if (response.status == 401) {
                console.log("sometyhing went wrong {401}");
                authorizated = false;
                token = "";
                loginFormVisible(true);
                authFormVisible(false);
            }
        })
        .then(data => {
            console.log("----------------");
            console.log(data);
            console.log(data.accessToken);
            console.log(data.refreshToken);
            console.log("----------------");

            token.accessToken = data.accessToken;
            token.refreshToken = data.refreshToken;
            authorizated = true;

            loginFormVisible(false);
            authFormVisible(true);

        }).catch(error => console.log(error));
}

function loginFormVisible(bool) {
    if (bool) {
        document.getElementById("loginForm").style.display = "block";
    } else {
        document.getElementById("loginForm").style.display = "none";
    }
}

function authFormVisible(bool) {
    if (bool) {
        document.getElementById("authForm").style.display = "block";
    } else {
        document.getElementById("authForm").style.display = "none";
    }
}

document.getElementById("showUsers").onclick = () => {
    showUsers(token);
}

async function refresh(func) {
    const response = await fetch("https://localhost:7174/api/users/refresh", {
        method: "post",
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify(token)
    });

    const result = await response.json();

    token = result;

    console.log("----------------");
    console.log(result);
    console.log(result.accessToken);
    console.log(result.refreshToken);
    console.log("----------------");

    showUsers(result);
}

async function showUsers(token) {
    const response = await fetch("https://localhost:7174/api/users", {
        method: "get",
        headers: {
            "Authorization": `Bearer ${token.accessToken}`
        }
    });

    if (response.ok) {
        let data = await response.json();
        alert(data);
    }
    else if (response.status == 401) {
        console.log("expired");
        await refresh();
    }
    else {
        console.log("can not get users info ???");
    }
}
