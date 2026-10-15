const PALETTE = [
  'linear-gradient(135deg, #1f6f52, #0e2e24)',
  'linear-gradient(135deg, #3b3f57, #171a24)',
  'linear-gradient(135deg, #5b4a1e, #211b0c)',
  'linear-gradient(135deg, #4a2e5e, #1b1124)',
  'linear-gradient(135deg, #1e4a5b, #0c1d24)',
];

export function coverArt(seed: string): string {
  let hash = 0;
  for (const ch of seed) {
    hash = (hash * 31 + ch.charCodeAt(0)) | 0;
  }
  return PALETTE[Math.abs(hash) % PALETTE.length];
}
