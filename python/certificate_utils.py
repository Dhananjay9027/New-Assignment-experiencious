from PIL import Image, ImageDraw, ImageFont
import smtplib
from email.message import EmailMessage
from datetime import datetime
from dotenv import load_dotenv
import os

# Load .env values
load_dotenv()

# Read email credentials from environment variables
EMAIL_ADDRESS = os.getenv("EMAIL_ADDRESS")
EMAIL_PASSWORD = os.getenv("EMAIL_PASSWORD")

def generate_certificate(name: str, output_path: str = "certificate.png"):
    try:
        template = Image.open("base_certificate.png").convert("RGBA")
        draw = ImageDraw.Draw(template)

        try:
            font = ImageFont.truetype("arial.ttf", 80)  # Try system font
        except OSError:
            font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf", 80)

        # Calculate text bounding box
        bbox = draw.textbbox((0, 0), name, font=font)
        text_width = bbox[2] - bbox[0]
        text_height = bbox[3] - bbox[1]

        # Center position
        x = (template.width - text_width) / 2
        y = template.height // 2

        # Draw white semi-transparent background for text
        draw.rectangle(
            [x - 20, y - 10, x + text_width + 20, y + text_height + 10],
            fill=(255, 255, 255, 200)
        )

        # Draw name
        draw.text((x, y), name, fill="black", font=font)

        template.save(output_path)
        print(f" Certificate saved as {output_path}")
        return output_path

    except Exception as e:
        raise Exception(f" Certificate generation failed: {e}")

def send_certificate_email(email_to: str, name: str):
    try:
        print(f"📬 Sending certificate to {email_to} for {name}")
        cert_path = generate_certificate(name)

        msg = EmailMessage()
        msg['Subject'] = '🎓 Your Participation Certificate'
        msg['From'] = EMAIL_ADDRESS
        msg['To'] = email_to
        msg.set_content(f"Hi {name},\n\nThanks for participating! Your certificate is attached.")

        with open(cert_path, 'rb') as f:
            msg.add_attachment(f.read(), maintype='image', subtype='png', filename="certificate.png")

        with smtplib.SMTP_SSL('smtp.gmail.com', 465) as smtp:
            smtp.login(EMAIL_ADDRESS, EMAIL_PASSWORD)
            smtp.send_message(msg)

        print(" Certificate email sent successfully.")

    except Exception as e:
        raise Exception(f" Failed to send certificate email: {e}")
