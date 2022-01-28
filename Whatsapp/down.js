async function downloadImage() {
  const image = await fetch('url')
  const imageBlog = await image.blob()
  const imageURL = URL.createObjectURL(imageBlog)

  const link = document.createElement('a')
  link.href = imageURL
  link.download = 'nome'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}downloadImage();