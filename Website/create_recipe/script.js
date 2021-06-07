"use strict";

function openSidebar() {
  document.getElementById("side_menu").style.width = "19%";
}

function closeSidebar() {
  document.getElementById("side_menu").style.width = "0";
}

function hideProductDetails() {
    document.getElementById("product_details").style.display="none";
}

function showProductDetails() {
    document.getElementById("product_details").style.display="inline-block";
}