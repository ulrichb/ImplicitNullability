package com.jetbrains.rider.settings

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class ImplicitNullabilityOptionsPage : SimpleOptionsPage("Implicit Nullability", ImplicitNullabilityOptionsPage::class.simpleName!!) {

    override fun getId(): String {
        return "preferences." + this.pageId;
    }
}
