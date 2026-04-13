"""
generate_greeting_wav.py
Generates assets/greeting.wav – a simple spoken-style chime using pure Python
(no external libraries needed). The file is a valid 16-bit PCM WAV.

Run once before building CyberBot:
    python generate_greeting_wav.py
"""

import math
import struct
import wave
import os

SAMPLE_RATE = 44100

def sine_wave(freq: float, duration: float, amplitude: float = 0.5) -> list[int]:
    """Return a list of 16-bit PCM samples for a sine tone."""
    n_samples = int(SAMPLE_RATE * duration)
    samples = []
    for i in range(n_samples):
        t = i / SAMPLE_RATE
        # Apply a simple fade-in / fade-out envelope
        env = min(i / (SAMPLE_RATE * 0.02), 1.0, (n_samples - i) / (SAMPLE_RATE * 0.05))
        sample = amplitude * env * math.sin(2 * math.pi * freq * t)
        samples.append(int(sample * 32767))
    return samples

def silence(duration: float) -> list[int]:
    return [0] * int(SAMPLE_RATE * duration)

# Build a cheerful ascending chime: C5 – E5 – G5 – C6
notes = [
    (523.25, 0.18),   # C5
    (659.25, 0.18),   # E5
    (783.99, 0.18),   # G5
    (1046.50, 0.35),  # C6
]

samples: list[int] = []
samples += silence(0.1)
for freq, dur in notes:
    samples += sine_wave(freq, dur, amplitude=0.45)
    samples += silence(0.04)
samples += silence(0.15)

# Write WAV
out_path = os.path.join(os.path.dirname(__file__), "CyberBot", "assets", "greeting.wav")
os.makedirs(os.path.dirname(out_path), exist_ok=True)

with wave.open(out_path, "w") as wf:
    wf.setnchannels(1)
    wf.setsampwidth(2)          # 16-bit
    wf.setframerate(SAMPLE_RATE)
    packed = struct.pack(f"<{len(samples)}h", *samples)
    wf.writeframes(packed)

print(f"✅  Generated: {out_path}  ({len(samples) / SAMPLE_RATE:.2f}s)")
