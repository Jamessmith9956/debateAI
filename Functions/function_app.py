import azure.functions as func
import logging
from youtube_transcript_api import YouTubeTranscriptApi
from youtube_transcript_api.formatters import JSONFormatter

app = func.FunctionApp(http_auth_level=func.AuthLevel.ANONYMOUS)

@app.route(route="extract_yt_transcript")
def extract_yt_transcript(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    id = req.params.get('id')
    if not id:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            id = req_body.get('id')

    if id:
        formatter = JSONFormatter()
        data = YouTubeTranscriptApi.get_transcript(id)
        json = formatter.format_transcript(data)
        return func.HttpResponse(f"HTTP triggered function for video ID:({id}) executed successfully. \n here is the json: \n {json}")
    else:
        return func.HttpResponse(
             "This HTTP triggered function executed successfully. Pass an ID in the query string or in the request body for to retrieve a YT transcript.",
             status_code=200
        )