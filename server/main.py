from flask import Flask, after_this_request, make_response, send_file
from flask import request
import os
from flask_cors import CORS, cross_origin
import zipfile

app = Flask(__name__)
app.config["UPLOAD_FOLDER"] = "files/"
cors = CORS(app)  # allow CORS for all domains on all routes.
app.config["CORS_HEADERS"] = "Content-Type"


@app.route("/")
def hello_world():
    print("worked")
    return "hello world"


@app.route("/uploadfile", methods=["POST"])
@cross_origin()
def uploadFile():
    # check if the post request has the file part
    if "file" not in request.files:
        return make_response("send file bruv", 400)
    file = request.files["file"]
    # If the user does not select a file, the browser submits an
    # empty file without a filename.
    if file.filename == "":
        return make_response("no selected file", 400)
    file.save(os.path.join(app.config["UPLOAD_FOLDER"], "breadboard.kicad_pcb"))
    print("got file")
    os.system(
        "bash ../conversion/kicadPcbToFbx.sh "
        + os.path.join(app.config["UPLOAD_FOLDER"], "breadboard.kicad_pcb")
        + " files"
    )
    os.remove(os.path.join(app.config["UPLOAD_FOLDER"], "breadboard.kicad_pcb"))
    print("converted file")
    return make_response("worked fine", 200)


@app.route("/getfile", methods=["GET"])
@cross_origin()
def getFile():
    zipf = zipfile.ZipFile("output.zip", "w", zipfile.ZIP_DEFLATED)
    if len(os.listdir(app.config["UPLOAD_FOLDER"])) == 0:
        return make_response("not yet", 200)

    for root, dirs, files in os.walk(app.config["UPLOAD_FOLDER"]):
        for file in files:
            zipf.write(app.config["UPLOAD_FOLDER"] + file)
            os.remove(app.config["UPLOAD_FOLDER"] + file)
    zipf.close()

    @after_this_request
    def deleteZip(response):
        os.remove("output.zip")
        return response

    return send_file(
        "output.zip",
        mimetype="zip",
    )
