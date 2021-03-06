﻿using AzureServicesForUnity.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace AzureServicesForUnity.Shared
{
    public static class Utilities
    {
        public static void ValidateForNull(params object[] objects)
        {
            foreach (object obj in objects)
            {
                if (obj == null)
                {
                    throw new Exception("Argument null");
                }
            }

        }

        public static bool IsWWWError(UnityWebRequest www)
        {
            return www.isError || (www.responseCode >= 400L && www.responseCode <= 511L);
        }

        public static void BuildResponseObjectOnFailure(CallbackResponse response, UnityWebRequest www)
        {
            if (www.responseCode == 404L)
                response.Status = CallBackResult.NotFound;
            else if (www.responseCode == 409L)
                response.Status = CallBackResult.ResourceExists;
            else
                response.Status = CallBackResult.Failure;

            string errorMessage = www.error;
            if (errorMessage == null && www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                errorMessage = www.downloadHandler.text;
            else
                errorMessage = Globals.ErrorOccurred;

            Exception ex = new Exception(errorMessage);
            response.Exception = ex;
        }

        public static void BuildResponseObjectOnException(CallbackResponse response, Exception ex)
        {
            response.Status = CallBackResult.LocalException;
            response.Exception = ex;
        }


    }

    //http://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/#post-2585129
    public class JsonHelper
    {
        public static T[] GetJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array = null;
        }

        [Serializable]
        private class TableStorageResult<T>
        {
            public T[] value = null;
        }

        public static T[] GetJsonArrayFromTableStorage<T>(string json)
        {
            TableStorageResult<T> result = JsonUtility.FromJson<TableStorageResult<T>>(json);
            return result.value;
        }
    }

    public enum HttpMethod
    {
        Post,
        Get,
        Patch,
        Delete,
        Put,
        Merge
    }
}

