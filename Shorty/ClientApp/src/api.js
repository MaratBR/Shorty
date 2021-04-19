export class ApiError extends Error {
    constructor(errors) {
        super();
        this.errors = errors
    }
}

export async function getShortenedLink(link) {
    const response = await fetch('/shorten', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({link})
    })
    let json = await response.json();
    if (response.status === 200) {
        return json
    } else if (response.status >= 400 && response.status < 600) {
        if (json.errors)
            json = json.errors;
        throw new ApiError(json)
    }
}