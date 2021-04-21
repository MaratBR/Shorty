export class ApiError extends Error {
    constructor(errors) {
        super();
        this.errors = errors
    }
}

async function _fetch(input, init) {
    const response = await fetch(input, init)
    const body = await response.json()
    if (response.status >= 200 && response.status <= 299) {
        return body
    }
    if (body.errors && body.errors instanceof Array)
        throw new ApiError(body.errors)
    throw new ApiError(body)
}

export async function getShortenedLink(link) {
    return await _fetch('/shorten', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({link})
    })
}

export async function getStats() {
    return await _fetch('/stats')
}