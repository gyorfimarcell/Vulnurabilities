import random
import win32file
import win32pipe

PIPE_NAME = r"\\.\pipe\vuln"

LABELS = [
    "Spoofing",
    "Tampering",
    "Repudiation",
    "Information disclosure",
    "Denial of service",
    "Elevation of privilege",
]


def pipe_server():
    print("Waiting for client...")

    while True:
        pipe = win32pipe.CreateNamedPipe(
            PIPE_NAME,
            win32pipe.PIPE_ACCESS_DUPLEX,
            win32pipe.PIPE_TYPE_MESSAGE
            | win32pipe.PIPE_READMODE_MESSAGE
            | win32pipe.PIPE_WAIT,
            1,  # Number of instances
            16384,  # Output buffer size
            16384,  # Input buffer size
            0,  # Default timeout
            None,  # No security attributes
        )

        win32pipe.ConnectNamedPipe(pipe, None)
        print("C# client connected.")

        try:
            while True:
                # Read message from the client
                hr, message = win32file.ReadFile(pipe, 16384)
                message = message.decode().strip()
                if not message:
                    break

                print(f'Processing "{message}"')

                response = f"{random.choice(LABELS)}\n"
                win32file.WriteFile(pipe, response.encode())
        except Exception as e:
            print(f"Error: {e}")
        finally:
            win32file.CloseHandle(pipe)
            print("Client disconnected, waiting for a new client...")


pipe_server()
