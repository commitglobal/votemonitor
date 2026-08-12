import { toast } from 'sonner'

/**
 * Presentation properties that have to travel with the exported drawing.
 *
 * Everything a chart looks like comes from stylesheets, and a serialized SVG
 * carries none of them, so each one is read off the live element and written
 * onto the copy as an attribute.
 */
const PAINT_PROPERTIES = [
  'fill',
  'fill-opacity',
  'stroke',
  'stroke-width',
  'stroke-opacity',
  'stroke-dasharray',
  'stroke-linecap',
  'stroke-linejoin',
  'opacity',
  'font-family',
  'font-size',
  'font-weight',
  'text-anchor',
  'dominant-baseline',
] as const

/** Exported at twice the on-screen size, so the file stays sharp when zoomed. */
const EXPORT_SCALE = 2

/**
 * Saves the chart inside `container` as a PNG.
 *
 * Recharts draws with CSS custom properties (`var(--color-…)`), which only
 * resolve while the drawing is attached to the document. Cloning the SVG and
 * copying the *computed* paint of every node freezes those colours into the
 * copy; without this step the export comes out black.
 */
export async function downloadChartAsPng(
  container: HTMLElement | null,
  fileName: string
): Promise<void> {
  const source = container?.querySelector('svg')

  if (!source) {
    toast.error('Nothing to download', {
      description: 'The chart has not finished rendering yet.',
    })
    return
  }

  try {
    const clone = source.cloneNode(true) as SVGSVGElement

    // Both trees are walked in the same order, so index N in one is the same
    // node as index N in the other.
    const sourceNodes = source.querySelectorAll<SVGElement>('*')
    const cloneNodes = clone.querySelectorAll<SVGElement>('*')

    sourceNodes.forEach((node, index) => {
      const target = cloneNodes[index]
      if (!target) {
        return
      }

      const computed = window.getComputedStyle(node)

      for (const property of PAINT_PROPERTIES) {
        const value = computed.getPropertyValue(property)
        if (value) {
          target.setAttribute(property, value)
        }
      }
    })

    const { width, height } = source.getBoundingClientRect()

    clone.setAttribute('xmlns', 'http://www.w3.org/2000/svg')
    clone.setAttribute('width', String(width))
    clone.setAttribute('height', String(height))

    // Charts are transparent by design. A PNG pasted into a document needs an
    // opaque ground, otherwise it picks up whatever sits behind it. Inserted
    // after the paint copy so it does not shift the node indices above.
    const surface =
      window.getComputedStyle(document.body).backgroundColor || '#ffffff'
    const background = document.createElementNS(
      'http://www.w3.org/2000/svg',
      'rect'
    )
    background.setAttribute('width', '100%')
    background.setAttribute('height', '100%')
    background.setAttribute('fill', surface)
    clone.insertBefore(background, clone.firstChild)

    const serialized = new XMLSerializer().serializeToString(clone)
    const svgUrl = `data:image/svg+xml;charset=utf-8,${encodeURIComponent(serialized)}`

    const image = new Image()
    await new Promise<void>((resolve, reject) => {
      image.onload = () => resolve()
      image.onerror = () => reject(new Error('Could not rasterize the chart'))
      image.src = svgUrl
    })

    const canvas = document.createElement('canvas')
    canvas.width = width * EXPORT_SCALE
    canvas.height = height * EXPORT_SCALE

    const context = canvas.getContext('2d')
    if (!context) {
      throw new Error('Canvas is unavailable')
    }

    context.scale(EXPORT_SCALE, EXPORT_SCALE)
    context.drawImage(image, 0, 0)

    const blob = await new Promise<Blob | null>((resolve) =>
      canvas.toBlob(resolve, 'image/png')
    )

    if (!blob) {
      throw new Error('Could not encode the image')
    }

    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = fileName
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    URL.revokeObjectURL(url)
  } catch {
    toast.error('Download failed', {
      description: 'The chart could not be saved as an image.',
    })
  }
}
