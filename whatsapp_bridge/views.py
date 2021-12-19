from django.shortcuts import render
from django.http import HttpResponse
from django.views.decorators.csrf import csrf_exempt

# Create your views here.
@csrf_exempt
def index(request):
    print('POST_KEYS',request.POST)
    return HttpResponse("Hello world. You're at the whatsapp bridge")

