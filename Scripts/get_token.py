from oauthlib.oauth2 import WebApplicationClient
from requests.auth import HTTPBasicAuth
from requests_oauthlib import OAuth2Session

# Replace these with your actual values
client_id = 'client_id'
client_secret = 'client_secret'
token_url = 'https://oauth2.googleapis.com/token'
authorization_base_url = 'https://accounts.google.com/o/oauth2/auth'
token_url = 'https://oauth2.googleapis.com/token'
scope = 'https://www.googleapis.com/auth/youtube.force-ssl'
redirect_uri = 'https://localhost:8080'

# Set up OAuth client
client = WebApplicationClient(client_id)
oauth = OAuth2Session(client=client, redirect_uri=redirect_uri, scope=scope)

# Get authorization URL and open it in a web browser
authorization_url, state = oauth.authorization_url(authorization_base_url)
print(f'Please go to {authorization_url} and authorize the application.')
redirect_response = input('Paste the full redirect URL here: ')

# Get access token using the authorization code
token = oauth.fetch_token(
    token_url=token_url,
    authorization_response=redirect_response,
    auth=HTTPBasicAuth(client_id, client_secret)
)

print("Access Token:", token)
