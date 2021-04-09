export async function getShortenedLink(link) {
    const response = await fetch('/shorten', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({link})
    })
    if (response.status === 200) {
        const {linkId} = await response.json()
        return linkId
    }
    return null
}