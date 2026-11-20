// What the nodes say of themselves in more than one language: the description
// of a station login, a login group, a connection and a set of credentials is
// such a text on the wire - {"en": "..."} - and a page shows one of its texts
// and sends what was typed into one field as the English one.

/** A text in as many languages as it is said in: by the code of its language, as "en". */
export type I18NString = Partial<Record<string, string>>;

/** The text of a description to show: the English one, or the first there is. */
export function textOf(text?: I18NString): string | undefined {
    if (!text)
        return undefined;
    return text['en'] ?? Object.values(text).find(value => value !== undefined);
}

/** What somebody typed into one field, as a description in English; none for nothing. */
export function inEnglish(text: string): I18NString | undefined {
    return text.trim().length > 0 ? { en: text.trim() } : undefined;
}
