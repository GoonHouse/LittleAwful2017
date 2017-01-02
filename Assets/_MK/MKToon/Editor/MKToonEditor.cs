using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace MKToon
{
    public class MKToonEditor : ShaderGUI
    {
        private enum LightMode
        {
            DEFAULT,
            TOON_TRESHOLD,
            TOON_LEVELS
        }

        private MaterialProperty color = null;
        private MaterialProperty brightness = null;
        private MaterialProperty mainTex = null;
        private MaterialProperty normalMap = null;
        private MaterialProperty bumpiness = null;
        private MaterialProperty lldiffuse = null;
        private MaterialProperty llspecular = null;
        private MaterialProperty lTreshold = null;
        private MaterialProperty shininess = null;
        private MaterialProperty specularColor = null;
        private MaterialProperty toonyFy = null;
        private MaterialProperty rimColor = null;
        private MaterialProperty rimSize = null;
        private MaterialProperty shadowColor = null;
        private MaterialProperty smoothness = null;
        private MaterialProperty cullMode = null;
        private MaterialProperty outlineColor = null;
        private MaterialProperty outlineSize = null;

        private MaterialProperty useNormalMap = null;
        private MaterialProperty useRim = null;
        private MaterialProperty useSpecular = null;
        private MaterialProperty useCustomShadow = null;
        private MaterialProperty lightMode = null;
        private MaterialProperty useMTex = null;

        public void FindProperties(MaterialProperty[] props, Material mat)
        {
            color = FindProperty("_Color", props);
            brightness = FindProperty("_Brightness", props);
            mainTex = FindProperty("_MainTex", props);
            normalMap = FindProperty("_NormalMap", props);
            bumpiness = FindProperty("_Bumpiness", props);
            lldiffuse = FindProperty("_LightLevelsDiffuse", props);
            llspecular = FindProperty("_LightLevelsSpecular", props);
            lTreshold = FindProperty("_LThreshold", props);
            shininess = FindProperty("_Shininess", props);
            specularColor = FindProperty("_SpecularColor", props);
            toonyFy = FindProperty("_ToonyFy", props);
            rimColor = FindProperty("_RimColor", props);
            rimSize = FindProperty("_RimSize", props);
            shadowColor = FindProperty("_ShadowColor", props);
            smoothness = FindProperty("_Smoothness", props);
            cullMode = FindProperty("_CullMode", props);
            if (mat.shader.name.Contains("Outline"))
            {
                outlineColor = FindProperty("_OutlineColor", props);
                outlineSize = FindProperty("_OutlineSize", props);
            }
            useNormalMap = FindProperty("_UseNormalMap", props);
            useRim = FindProperty("_UseRim", props);
            useSpecular = FindProperty("_UseSpecular", props);
            useCustomShadow = FindProperty("_UseCustomShadow", props);
            useMTex = FindProperty("_UseMTex", props);
            lightMode = FindProperty("_LightMode", props);
        }

        override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material targetMat = materialEditor.target as Material;
            string[] keyWords = targetMat.shaderKeywords;
            FindProperties(properties, targetMat);
            EditorGUI.BeginChangeCheck();
            materialEditor.ShaderProperty(cullMode, cullMode.displayName);
            materialEditor.ShaderProperty(color, color.displayName);
            materialEditor.ShaderProperty(brightness, brightness.displayName);
            materialEditor.ShaderProperty(useMTex, useMTex.displayName);
            if(useMTex.floatValue == 1)
            materialEditor.ShaderProperty(mainTex, mainTex.displayName);
            Divider();

            materialEditor.ShaderProperty(useNormalMap, useNormalMap.displayName);

            if (useNormalMap.floatValue == 1)
            {
                materialEditor.ShaderProperty(normalMap, normalMap.displayName);
                materialEditor.ShaderProperty(bumpiness, bumpiness.displayName);
            }
            Divider();

            materialEditor.ShaderProperty(useSpecular, useSpecular.displayName);
            if (useSpecular.floatValue == 1)
            {
                materialEditor.ShaderProperty(shininess, shininess.displayName);
                materialEditor.ShaderProperty(specularColor, specularColor.displayName);
            }
            Divider();

            materialEditor.ShaderProperty(useRim, useRim.displayName);
            if (useRim.floatValue == 1)
            {
                materialEditor.ShaderProperty(rimColor, rimColor.displayName);
                materialEditor.ShaderProperty(rimSize, rimSize.displayName);
            }
            Divider();

            materialEditor.ShaderProperty(useCustomShadow, useCustomShadow.displayName);
            if (useCustomShadow.floatValue == 1)
            {
                materialEditor.ShaderProperty(shadowColor, shadowColor.displayName);
            }
            Divider();

            EditorGUILayout.LabelField("Render Settings", EditorStyles.boldLabel);
            materialEditor.ShaderProperty(toonyFy, toonyFy.displayName);
            if (lightMode.floatValue != 0)
                materialEditor.ShaderProperty(smoothness, smoothness.displayName);

            Divider();
            EditorGUILayout.LabelField("Light", EditorStyles.boldLabel);
            materialEditor.ShaderProperty(lightMode, lightMode.displayName);
            if(lightMode.floatValue == 1)
                    materialEditor.ShaderProperty(lTreshold, lTreshold.displayName);
            if(lightMode.floatValue == 2)
            { 
                materialEditor.ShaderProperty(lldiffuse, lldiffuse.displayName);
                if (useSpecular.floatValue == 1)
                {
                    materialEditor.ShaderProperty(llspecular, llspecular.displayName);
                }
            }

            if (targetMat.shader.name.Contains("Outline"))
            {
                Divider();
                EditorGUILayout.LabelField("Outline", EditorStyles.boldLabel);
                materialEditor.ShaderProperty(outlineColor, outlineColor.displayName);
                materialEditor.ShaderProperty(outlineSize, outlineSize.displayName);
            }

            EditorGUI.EndChangeCheck();
        }
        private void Divider()
        {
            GUILayout.Space(12);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }
    }
}