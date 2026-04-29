const promptForgeLinks = {
  version: "5.1.1",
  installerName: "PromptForge-5.1.1-Setup.exe",
  installer:
    "https://github.com/windysoliloquy/PromptForgeV2/releases/download/v5.1.1/PromptForge-5.1.1-Setup.exe",
  release: "https://github.com/windysoliloquy/PromptForgeV2/releases/latest",
  supportEmail: "mailto:WindySoliloquy@gmail.com",
};

const promptForgeBackgroundPanels = {
  topLeft: "/assets/backpanel1L.png",
  topRight: "/assets/backpanel1R.png",
  bottomLeft: "/assets/backpanel2L.png",
  bottomRight: "/assets/backpanel2R.png",
};

const promptForgeMiddleBackgroundPanels = {
  first: "/assets/test1.png",
  second: "/assets/test2.png",
};

const promptForgeGallery = [
  {
    eyebrow: "WORKSTATION OVERVIEW",
    slug: "ui-overview-main",
    title: "Main App View",
    description: "The main app view keeps the workflow readable while holding deeper prompt controls in collapsed sections instead of dumping every precision element on the screen at once.",
    image: "/assets/UI.png",
    alt: "Prompt Forge HoverDeck overview showing the intro card, intent section, subject box, and collapsed sections.",
    featured: true,
    imageClass: "contain-top",
  },
  {
    eyebrow: "PRECISION CONTROLS",
    slug: "ui-controls-depth",
    title: "Structured control over style, mood, lighting, and composition",
    description: "Expanded controls reveal Prompt Forge's precision layer: camera controls, complexity, lighting, framing, and 20+ other prompt tools most users do not even know they can leverage, all ready at the touch of a button.",
    image: "/assets/controls%20within%20the%20UI.png",
    alt: "Prompt Forge expanded controls showing Style Controls, Mood, Control Lighting, Image Finish, and Scene Composition.",
    imageClass: "contain-top",
  },
  {
    eyebrow: "ARTIST INFLUENCE",
    slug: "ui-artist-influence",
    title: "Artist influence stays integrated with the full prompt workflow",
    description: "People already add artists to prompts. Prompt Forge gives those artist choices a proper control surface, with pairing tools beside the artists so blended influence can be shaped instead of guessed.",
    image: "/assets/Artist%20Influence%20Panel.png",
    alt: "Prompt Forge Artist Influence panel shown as part of the broader prompt-building workflow.",
    imageClass: "contain-center",
  },
  {
    eyebrow: "PAIR GUIDANCE",
    slug: "ui-artist-pair-guidance",
    title: "Artist pairing guidance helps mashups behave more smoothly",
    description: "The Artist Pair Guidance tooltip only appears after you select two artist styles for the same image. If you want to stop exploring mashups, use the table-flip controls and turn each artist slider off safely. It takes real semantic work to make artist blends behave, and the off-ramp is still under development.",
    image: "/assets/artist%20tool.png",
    alt: "Prompt Forge artist tool guidance shown beside the Artist Influence workflow.",
    imageClass: "contain-center",
  },
];

const promptForgeLaneProof = [
  {
    title: "Lane-system proof overview",
    description:
      "Prompt Forge maps image direction through a lane system built for distinct subdomains, style families, and controlled prompt behavior rather than loose phrase stacking.",
    image: "/assets/Lane-Proof1.png",
    alt: "Prompt Forge lane-system overview proof image showing the breadth of the controlled lane structure.",
  },
  {
    title: "Distinct subdomains and style paths",
    description:
      "The lane system is meant to hold real variation: different paths, different visual territories, and prompt language that changes with the lane instead of staying generic.",
    image: "/assets/Lane-Proof2.png",
    alt: "Prompt Forge lane-system proof image showing distinct subdomains and style-path coverage.",
  },
  {
    title: "Prompt language shaped by lane behavior",
    description:
      "Prompt Forge does not treat every style the same. Each lane carries its own visual logic so the prompt reflects the lane instead of flattening everything into one generic voice.",
    image: "/assets/Lane-Proof3.jpg",
    alt: "Prompt Forge lane-system proof image showing style-specific prompt behavior across the lane set.",
  },
  {
    title: "Controlled range across the workstation",
    description:
      "The lane system is broad by design, but still structured enough to feel like one instrument instead of a disconnected pile of preset categories.",
    image: "/assets/lane-proof4.png",
    alt: "Prompt Forge lane-system proof image showing the controlled breadth of the workstation.",
  },
];

const promptForgeValueCarousel = [
  {
    image: "/assets/WorkFlow1.png",
    alt: "Prompt Forge workflow image 1 showing the first step in the lane-guided tree workflow.",
    caption: "Workflow step 1 for the ArchTree result, showing the first prompt-shaping move inside Prompt Forge.",
    lightboxGroup: "workflow-proof",
    prevImage: "/assets/WorkFlow5.png",
    nextImage: "/assets/WorkFlow2.png",
  },
  {
    image: "/assets/WorkFlow2.png",
    alt: "Prompt Forge workflow image 2 showing the second step in the lane-guided tree workflow.",
    caption: "Workflow step 2 for the ArchTree result, continuing the image-path decisions with only a few clicks.",
    lightboxGroup: "workflow-proof",
    prevImage: "/assets/WorkFlow1.png",
    nextImage: "/assets/WorkFlow3.png",
  },
  {
    image: "/assets/WorkFlow3.png",
    alt: "Prompt Forge workflow image 3 showing the third step in the lane-guided tree workflow.",
    caption: "Workflow step 3 for the ArchTree result, refining the direction without waiting on an LLM-written prompt.",
    lightboxGroup: "workflow-proof",
    prevImage: "/assets/WorkFlow2.png",
    nextImage: "/assets/WorkFlow4.png",
  },
  {
    image: "/assets/WorkFlow4.png",
    alt: "Prompt Forge workflow image 4 showing the fourth step in the lane-guided tree workflow.",
    caption: "Workflow step 4 for the ArchTree result, staying inside the visual workflow instead of chat drafting.",
    lightboxGroup: "workflow-proof",
    prevImage: "/assets/WorkFlow3.png",
    nextImage: "/assets/WorkFlow5.png",
  },
  {
    image: "/assets/WorkFlow5.png",
    alt: "Prompt Forge workflow image 5 showing the final step in the lane-guided tree workflow.",
    caption: "Workflow step 5 for the ArchTree result, the last visible step before landing on the finished image.",
    lightboxGroup: "workflow-proof",
    prevImage: "/assets/WorkFlow4.png",
    nextImage: "/assets/WorkFlow1.png",
  },
];

const promptForgeSandcastleCarousel = [
  {
    image: "/assets/sandcastle/1.png",
    alt: "Sandcastle proof image 1 in the staged concept progression.",
    caption: "Sandcastle proof image 1.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/sword%20and%20sorcery.png",
    nextImage: "/assets/sandcastle/general%20fantasy.png",
  },
  {
    image: "/assets/sandcastle/general%20fantasy.png",
    alt: "Sandcastle general fantasy image in the staged concept progression.",
    caption: "Sandcastle proof image: general fantasy.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/1.png",
    nextImage: "/assets/sandcastle/2.png",
  },
  {
    image: "/assets/sandcastle/2.png",
    alt: "Sandcastle proof image 2 in the staged concept progression.",
    caption: "Sandcastle proof image 2.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/general%20fantasy.png",
    nextImage: "/assets/sandcastle/low%20magic.png",
  },
  {
    image: "/assets/sandcastle/low%20magic.png",
    alt: "Sandcastle low magic image in the staged concept progression.",
    caption: "Sandcastle proof image: low magic.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/2.png",
    nextImage: "/assets/sandcastle/3.png",
  },
  {
    image: "/assets/sandcastle/3.png",
    alt: "Sandcastle proof image 3 in the staged concept progression.",
    caption: "Sandcastle proof image 3.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/low%20magic.png",
    nextImage: "/assets/sandcastle/high%20magic.png",
  },
  {
    image: "/assets/sandcastle/high%20magic.png",
    alt: "Sandcastle high magic image in the staged concept progression.",
    caption: "Sandcastle proof image: high magic.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/3.png",
    nextImage: "/assets/sandcastle/4.png",
  },
  {
    image: "/assets/sandcastle/4.png",
    alt: "Sandcastle proof image 4 in the staged concept progression.",
    caption: "Sandcastle proof image 4.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/high%20magic.png",
    nextImage: "/assets/sandcastle/magitech.png",
  },
  {
    image: "/assets/sandcastle/magitech.png",
    alt: "Sandcastle magitech image in the staged concept progression.",
    caption: "Sandcastle proof image: magitech.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/4.png",
    nextImage: "/assets/sandcastle/5.png",
  },
  {
    image: "/assets/sandcastle/5.png",
    alt: "Sandcastle proof image 5 in the staged concept progression.",
    caption: "Sandcastle proof image 5.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/magitech.png",
    nextImage: "/assets/sandcastle/sword%20and%20sorcery.png",
  },
  {
    image: "/assets/sandcastle/sword%20and%20sorcery.png",
    alt: "Sandcastle sword and sorcery image in the staged concept progression.",
    caption: "Sandcastle proof image: sword and sorcery.",
    lightboxGroup: "sandcastle-proof",
    prevImage: "/assets/sandcastle/5.png",
    nextImage: "/assets/sandcastle/1.png",
  },
];

const promptForgeLightboxGroups = {
  "workflow-proof": promptForgeValueCarousel,
  "sandcastle-proof": promptForgeSandcastleCarousel,
};

const promptForgeLightboxRecordsByFilename = Object.fromEntries(
  [...promptForgeValueCarousel, ...promptForgeSandcastleCarousel].map((item, index) => [
    item.image.split("/").pop().toLowerCase(),
    {
      ...item,
      groupItems: item.lightboxGroup === "workflow-proof" ? promptForgeValueCarousel : promptForgeSandcastleCarousel,
      groupIndex: item.lightboxGroup === "workflow-proof"
        ? promptForgeValueCarousel.findIndex((entry) => entry.image === item.image)
        : promptForgeSandcastleCarousel.findIndex((entry) => entry.image === item.image),
    },
  ])
);

const getLightboxRecordFromSource = (source) => {
  const normalizedSource = (source || "").split("?")[0];
  const filename = normalizedSource.split("/").pop()?.toLowerCase();
  return filename ? promptForgeLightboxRecordsByFilename[filename] || null : null;
};

let promptForgeLaneProofTimer = null;
let promptForgeMiddleBackgroundHeight = 0;
const promptForgeMiddleBackgroundRate = 0.42;
const promptForgeMiddleBackgroundFooterOverscan = 240;

const createExpandableImageMarkup = (item, className = "") =>
  `<img class="${className}" src="${item.image}" alt="${item.alt}" loading="lazy"${item.caption ? ` data-lightbox-caption="${item.caption}"` : ""}${item.lightboxGroup ? ` data-lightbox-group="${item.lightboxGroup}"` : ""}${item.prevImage ? ` data-lightbox-prev-image="${item.prevImage}"` : ""}${item.nextImage ? ` data-lightbox-next-image="${item.nextImage}"` : ""}${Number.isInteger(item.lightboxIndex) ? ` data-lightbox-index="${item.lightboxIndex}"` : ""} data-expandable-image />`;

const ensureImageLightbox = () => {
  if (document.querySelector("[data-image-lightbox]")) {
    return;
  }

  const lightbox = document.createElement("div");
  lightbox.className = "image-lightbox";
  lightbox.setAttribute("data-image-lightbox", "true");
  lightbox.setAttribute("hidden", "");
  lightbox.innerHTML = `
    <div class="image-lightbox-backdrop" data-lightbox-close></div>
    <div class="image-lightbox-dialog" role="dialog" aria-modal="true" aria-label="Expanded image viewer">
      <button type="button" class="image-lightbox-close" data-lightbox-close aria-label="Close expanded image view">Close</button>
      <figure class="image-lightbox-figure">
        <img class="image-lightbox-image" alt="" />
        <figcaption class="image-lightbox-caption"></figcaption>
      </figure>
      <div class="image-lightbox-nav" hidden>
        <button type="button" class="button secondary image-lightbox-link" data-lightbox-nav="prev" aria-label="Show previous workflow image">Back</button>
        <button type="button" class="button secondary image-lightbox-link" data-lightbox-nav="next" aria-label="Show next workflow image">Next</button>
      </div>
    </div>
  `;

  document.body.appendChild(lightbox);
};

const activateExpandableImages = () => {
  ensureImageLightbox();

  const lightbox = document.querySelector("[data-image-lightbox]");
  const lightboxImage = lightbox.querySelector(".image-lightbox-image");
  const lightboxCaption = lightbox.querySelector(".image-lightbox-caption");
  const closeButton = lightbox.querySelector(".image-lightbox-close");
  const lightboxNav = lightbox.querySelector(".image-lightbox-nav");
  const navButtons = lightbox.querySelectorAll("[data-lightbox-nav]");
  let activeTrigger = null;
  const closeLightbox = () => {
    lightbox.setAttribute("hidden", "");
    document.body.classList.remove("lightbox-open");
    lightboxImage.removeAttribute("src");
    lightboxImage.alt = "";
    delete lightboxImage.dataset.prevImage;
    delete lightboxImage.dataset.nextImage;
    lightboxCaption.textContent = "";
    lightboxNav.setAttribute("hidden", "");

    if (activeTrigger) {
      activeTrigger.focus();
      activeTrigger = null;
    }
  };

  const syncLightboxNav = () => {
    const currentSource = (lightboxImage.src || "").split("?")[0].toLowerCase();
    if (currentSource.endsWith("/archtree.png")) {
      lightboxNav.setAttribute("hidden", "");
      return;
    }

    const prevItem = getLightboxRecordFromSource(lightboxImage.dataset.prevImage || "");
    const nextItem = getLightboxRecordFromSource(lightboxImage.dataset.nextImage || "");
    const hasGroup = !!(prevItem && nextItem);

    if (!hasGroup) {
      lightboxNav.setAttribute("hidden", "");
      return;
    }

    lightboxNav.removeAttribute("hidden");
    navButtons.forEach((button) => {
      const direction = button.dataset.lightboxNav;
      button.textContent = direction === "prev" ? "Back" : "Next";
      button.setAttribute(
        "aria-label",
        direction === "prev"
          ? `Show previous workflow image: ${prevItem?.alt || "workflow image"}`
          : `Show next workflow image: ${nextItem?.alt || "workflow image"}`
      );
    });
  };

  const openLightboxFromRecord = (record, trigger = null) => {
    if (trigger) {
      activeTrigger = trigger;
    }

    const captionText = record.caption || record.alt || "Expanded Prompt Forge proof image.";
    lightboxImage.src = record.image;
    lightboxImage.alt = record.alt || "";
    if (record.prevImage && record.nextImage) {
      lightboxImage.dataset.prevImage = record.prevImage;
      lightboxImage.dataset.nextImage = record.nextImage;
    } else {
      delete lightboxImage.dataset.prevImage;
      delete lightboxImage.dataset.nextImage;
    }
    lightboxCaption.textContent = captionText;
    syncLightboxNav();
    lightbox.removeAttribute("hidden");
    document.body.classList.add("lightbox-open");
    closeButton.focus();
  };

  const openLightbox = (image) => {
    if (image.dataset.lightboxStandalone === "true") {
      openLightboxFromRecord(
        {
          image: image.currentSrc || image.src,
          alt: image.alt || "",
          caption: image.dataset.lightboxCaption || image.alt || "Expanded Prompt Forge proof image.",
          prevImage: "",
          nextImage: "",
          groupItems: [],
          groupIndex: -1,
        },
        image
      );
      return;
    }

    const imageSource = (image.currentSrc || image.src || "").split("?")[0];
    const imageFilename = imageSource.split("/").pop()?.toLowerCase();
    if (imageFilename && promptForgeLightboxRecordsByFilename[imageFilename]) {
      const workflowItem = promptForgeLightboxRecordsByFilename[imageFilename];
      openLightboxFromRecord(
        {
          image: image.currentSrc || workflowItem.image,
          alt: workflowItem.alt,
          caption: workflowItem.caption,
          prevImage: workflowItem.prevImage,
          nextImage: workflowItem.nextImage,
          groupItems: workflowItem.groupItems,
          groupIndex: workflowItem.groupIndex,
        },
        image
      );
      return;
    }

    const lightboxGroup = image.dataset.lightboxGroup;
    const lightboxIndex = Number.parseInt(image.dataset.lightboxIndex || "", 10);
    if (lightboxGroup && Number.isInteger(lightboxIndex) && promptForgeLightboxGroups[lightboxGroup]?.[lightboxIndex]) {
      const groupItems = promptForgeLightboxGroups[lightboxGroup];
      const item = groupItems[lightboxIndex];
      openLightboxFromRecord(
        {
          image: image.currentSrc || item.image,
          alt: item.alt,
          caption: item.caption,
          prevImage: image.dataset.lightboxPrevImage || item.prevImage,
          nextImage: image.dataset.lightboxNextImage || item.nextImage,
          groupItems,
          groupIndex: lightboxIndex,
        },
        image
      );
      return;
    }

    openLightboxFromRecord(
      {
        image: image.currentSrc || image.src,
        alt: image.alt || "",
        caption: image.dataset.lightboxCaption || image.alt || "Expanded Prompt Forge proof image.",
        prevImage: image.dataset.lightboxPrevImage || "",
        nextImage: image.dataset.lightboxNextImage || "",
        groupItems: [],
        groupIndex: -1,
      },
      image
    );
  };

  document.querySelectorAll("[data-expandable-image]").forEach((image) => {
    if (image.dataset.expandableReady === "true") {
      return;
    }

    image.dataset.expandableReady = "true";
    image.tabIndex = 0;
    image.setAttribute("role", "button");
    image.setAttribute("aria-haspopup", "dialog");
    image.setAttribute("aria-label", `${image.alt || "Prompt Forge image"}. Open full-size view.`);

    image.addEventListener("click", () => openLightbox(image));
    image.addEventListener("keydown", (event) => {
      if (event.key === "Enter" || event.key === " ") {
        event.preventDefault();
        openLightbox(image);
      }
    });
  });

  lightbox.querySelectorAll("[data-lightbox-close]").forEach((element) => {
    if (element.dataset.lightboxCloseReady === "true") {
      return;
    }

    element.dataset.lightboxCloseReady = "true";
    element.addEventListener("click", closeLightbox);
  });

  navButtons.forEach((button) => {
    if (button.dataset.lightboxNavReady === "true") {
      return;
    }

    button.dataset.lightboxNavReady = "true";
    button.addEventListener("click", () => {
      const targetImage =
        button.dataset.lightboxNav === "prev"
          ? lightboxImage.dataset.prevImage || ""
          : lightboxImage.dataset.nextImage || "";
      const nextItem = getLightboxRecordFromSource(targetImage);
      if (!nextItem) {
        return;
      }
      openLightboxFromRecord({
        image: nextItem.image,
        alt: nextItem.alt,
        caption: nextItem.caption,
        prevImage: nextItem.prevImage,
        nextImage: nextItem.nextImage,
        groupItems: nextItem.groupItems,
        groupIndex: nextItem.groupIndex,
      });
    });
  });

  if (lightbox.dataset.lightboxKeybindReady !== "true") {
    lightbox.dataset.lightboxKeybindReady = "true";
    document.addEventListener("keydown", (event) => {
      if (event.key === "Escape" && !lightbox.hasAttribute("hidden")) {
        closeLightbox();
      }
    });
  }
};

const ensureSiteBackground = () => {
  if (document.querySelector("[data-site-background]")) {
    return;
  }

  const shell = document.createElement("div");
  shell.className = "site-background";
  shell.setAttribute("data-site-background", "true");
  shell.setAttribute("aria-hidden", "true");

  const track = document.createElement("div");
  track.className = "site-background-track";
  track.setAttribute("data-site-background-track", "true");

  const middle = document.createElement("div");
  middle.className = "site-background-middle";
  middle.setAttribute("data-site-background-middle", "true");

  const middleStack = document.createElement("div");
  middleStack.className = "site-background-middle-stack";
  middleStack.setAttribute("data-site-background-middle-stack", "true");
  middle.appendChild(middleStack);

  shell.appendChild(middle);
  shell.appendChild(track);
  document.body.prepend(shell);
};

ensureSiteBackground();

const measureSiteContentBottom = () => {
  const page = document.querySelector(".page");
  const footer = document.querySelector(".site-footer");
  const pageBottom = page ? page.offsetTop + page.offsetHeight : 0;
  const footerBottom = footer ? footer.offsetTop + footer.offsetHeight : 0;
  return Math.max(pageBottom, footerBottom);
};

const syncSiteBackgroundMiddleStack = () => {
  const middleStack = document.querySelector("[data-site-background-middle-stack]");
  if (!middleStack) {
    return 0;
  }

  const panelHeight = window.innerWidth * (2184 / 1228);
  const viewportHeight = window.innerHeight || document.documentElement.clientHeight;
  const scrollSpan = Math.max(document.documentElement.scrollHeight - viewportHeight, 0);
  const contentBottom = measureSiteContentBottom() + promptForgeMiddleBackgroundFooterOverscan;
  const targetHeight = Math.max(contentBottom, viewportHeight) + scrollSpan * promptForgeMiddleBackgroundRate + panelHeight * 2;

  promptForgeMiddleBackgroundHeight = targetHeight;
  middleStack.style.height = `${Math.round(promptForgeMiddleBackgroundHeight)}px`;
  middleStack.style.setProperty("--mid-panel-height", `${Math.round(panelHeight)}px`);
  middleStack.style.setProperty("--mid-image-1", `url("${promptForgeMiddleBackgroundPanels.first}")`);
  middleStack.style.setProperty("--mid-image-2", `url("${promptForgeMiddleBackgroundPanels.second}")`);
  return promptForgeMiddleBackgroundHeight;
};

document.querySelectorAll("[data-link]").forEach((element) => {
  const key = element.getAttribute("data-link");
  const url = promptForgeLinks[key];
  if (!url) {
    return;
  }

  if (element.tagName === "A") {
    element.href = url;
  }

  if (element.hasAttribute("data-text")) {
    element.textContent = url.replace(/^mailto:/, "");
  }
});

document.querySelectorAll("[data-version]").forEach((element) => {
  element.textContent = promptForgeLinks.version;
});

document.querySelectorAll("[data-installer-name]").forEach((element) => {
  element.textContent = promptForgeLinks.installerName;
});

document.querySelectorAll("[data-gallery-grid]").forEach((grid) => {
  promptForgeGallery.forEach((item) => {
    const card = document.createElement("article");
    card.className = `gallery-card${item.featured ? " featured" : ""}`;
    card.innerHTML = `
      <p class="eyebrow">${item.eyebrow}</p>
      <div class="gallery-media">
        ${createExpandableImageMarkup(item, item.imageClass || "")}
      </div>
      <div class="gallery-copy">
        <h3>${item.title}</h3>
        <p>${item.description}</p>
      </div>
    `;
    grid.appendChild(card);
  });

  activateExpandableImages();
});

document.querySelectorAll("[data-lane-proof-carousel]").forEach((mount) => {
  if (!promptForgeLaneProof.length) {
    return;
  }

  let currentIndex = Math.floor(Math.random() * promptForgeLaneProof.length);
  const slides = promptForgeLaneProof;

  const restartAutoplay = () => {
    if (promptForgeLaneProofTimer) {
      clearInterval(promptForgeLaneProofTimer);
    }

    if (slides.length < 2) {
      return;
    }

    promptForgeLaneProofTimer = setInterval(() => {
      currentIndex = (currentIndex + 1) % slides.length;
      render();
    }, 20000);
  };

  const render = () => {
    const item = slides[currentIndex];
    mount.innerHTML = `
      <div class="lane-proof-stage">
        <article class="lane-proof-slide" aria-label="Lane system proof slide ${currentIndex + 1} of ${slides.length}">
          <div class="lane-proof-media">
            ${createExpandableImageMarkup(item)}
          </div>
          <div class="lane-proof-copy">
            <p class="card-label muted">Lane-driven prompt behavior</p>
            <h3>${item.title}</h3>
            <p>${item.description}</p>
          </div>
        </article>
      </div>
    `;

    activateExpandableImages();

    if (slides.length < 2) {
      return;
    }

    const controls = document.createElement("div");
    controls.className = "lane-proof-controls";
    controls.innerHTML = `
      <button type="button" class="button secondary lane-proof-nav" data-direction="prev" aria-label="Show previous lane proof image">Previous</button>
      <div class="lane-proof-dots" role="tablist" aria-label="Lane proof slides"></div>
      <button type="button" class="button secondary lane-proof-nav" data-direction="next" aria-label="Show next lane proof image">Next</button>
    `;
    mount.appendChild(controls);

    const dots = controls.querySelector(".lane-proof-dots");
    slides.forEach((slide, index) => {
      const dot = document.createElement("button");
      dot.type = "button";
      dot.className = `lane-proof-dot${index === currentIndex ? " is-active" : ""}`;
      dot.setAttribute("aria-label", `Show lane proof image ${index + 1}`);
      dot.setAttribute("aria-pressed", index === currentIndex ? "true" : "false");
      dot.addEventListener("click", () => {
        currentIndex = index;
        render();
        restartAutoplay();
      });
      dots.appendChild(dot);
    });

    controls.querySelectorAll("[data-direction]").forEach((button) => {
      button.addEventListener("click", () => {
        currentIndex =
          button.getAttribute("data-direction") === "next"
            ? (currentIndex + 1) % slides.length
            : (currentIndex - 1 + slides.length) % slides.length;
        render();
        restartAutoplay();
      });
    });
  };

  render();
  restartAutoplay();
});

document.querySelectorAll("[data-value-carousel]").forEach((mount) => {
  if (!promptForgeValueCarousel.length) {
    return;
  }

  let currentIndex = 0;
  const slides = promptForgeValueCarousel;

  const render = () => {
    const item = slides[currentIndex];
    const itemIndex = currentIndex;
    mount.innerHTML = `
      <div class="value-carousel-shell">
        <div class="value-carousel-stage">
          <div class="value-carousel-slide" aria-label="Prompt Forge control proof ${currentIndex + 1} of ${slides.length}">
            <img src="${item.image}" alt="${item.alt}" loading="lazy"${item.caption ? ` data-lightbox-caption="${item.caption}"` : ""}${item.lightboxGroup ? ` data-lightbox-group="${item.lightboxGroup}"` : ""} data-lightbox-index="${itemIndex}" data-expandable-image />
          </div>
        </div>
      </div>
    `;

    if (slides.length < 2) {
      return;
    }

    const controls = document.createElement("div");
    controls.className = "value-carousel-controls";
    controls.innerHTML = `
      <button type="button" class="button secondary value-carousel-nav" data-direction="prev" aria-label="Show previous control proof image">Previous</button>
      <div class="value-carousel-dots" role="tablist" aria-label="Control proof slides"></div>
      <button type="button" class="button secondary value-carousel-nav" data-direction="next" aria-label="Show next control proof image">Next</button>
    `;
    mount.appendChild(controls);

    const dots = controls.querySelector(".value-carousel-dots");
    slides.forEach((slide, index) => {
      const dot = document.createElement("button");
      dot.type = "button";
      dot.className = `value-carousel-dot${index === currentIndex ? " is-active" : ""}`;
      dot.setAttribute("aria-label", `Show control proof image ${index + 1}`);
      dot.setAttribute("aria-pressed", index === currentIndex ? "true" : "false");
      dot.addEventListener("click", () => {
        currentIndex = index;
        render();
      });
      dots.appendChild(dot);
    });

    controls.querySelectorAll("[data-direction]").forEach((button) => {
      button.addEventListener("click", () => {
        currentIndex =
          button.getAttribute("data-direction") === "next"
            ? (currentIndex + 1) % slides.length
            : (currentIndex - 1 + slides.length) % slides.length;
        render();
      });
    });

    activateExpandableImages();
  };

  render();
});

document.querySelectorAll("[data-sandcastle-carousel]").forEach((mount) => {
  if (!promptForgeSandcastleCarousel.length) {
    return;
  }

  let currentIndex = 0;
  const slides = promptForgeSandcastleCarousel;

  const render = () => {
    const item = slides[currentIndex];
    const itemIndex = currentIndex;
    mount.innerHTML = `
      <div class="value-carousel-shell">
        <div class="value-carousel-stage">
          <div class="value-carousel-slide" aria-label="Sandcastle proof ${currentIndex + 1} of ${slides.length}">
            <img src="${item.image}" alt="${item.alt}" loading="lazy"${item.caption ? ` data-lightbox-caption="${item.caption}"` : ""}${item.lightboxGroup ? ` data-lightbox-group="${item.lightboxGroup}"` : ""}${item.prevImage ? ` data-lightbox-prev-image="${item.prevImage}"` : ""}${item.nextImage ? ` data-lightbox-next-image="${item.nextImage}"` : ""} data-lightbox-index="${itemIndex}" data-expandable-image />
          </div>
        </div>
      </div>
    `;

    if (slides.length < 2) {
      activateExpandableImages();
      return;
    }

    const controls = document.createElement("div");
    controls.className = "value-carousel-controls";
    controls.innerHTML = `
      <button type="button" class="button secondary value-carousel-nav" data-direction="prev" aria-label="Show previous sandcastle proof image">Previous</button>
      <div class="value-carousel-dots" role="tablist" aria-label="Sandcastle proof slides"></div>
      <button type="button" class="button secondary value-carousel-nav" data-direction="next" aria-label="Show next sandcastle proof image">Next</button>
    `;
    mount.appendChild(controls);

    const dots = controls.querySelector(".value-carousel-dots");
    slides.forEach((slide, index) => {
      const dot = document.createElement("button");
      dot.type = "button";
      dot.className = `value-carousel-dot${index === currentIndex ? " is-active" : ""}`;
      dot.setAttribute("aria-label", `Show sandcastle proof image ${index + 1}`);
      dot.setAttribute("aria-pressed", index === currentIndex ? "true" : "false");
      dot.addEventListener("click", () => {
        currentIndex = index;
        render();
      });
      dots.appendChild(dot);
    });

    controls.querySelectorAll("[data-direction]").forEach((button) => {
      button.addEventListener("click", () => {
        currentIndex =
          button.getAttribute("data-direction") === "next"
            ? (currentIndex + 1) % slides.length
            : (currentIndex - 1 + slides.length) % slides.length;
        render();
      });
    });

    activateExpandableImages();
  };

  render();
});

const heroParallaxSection = document.querySelector("[data-hero-parallax]");
const reduceMotionPreference = window.matchMedia("(prefers-reduced-motion: reduce)");

if (heroParallaxSection && !reduceMotionPreference.matches) {
  const baseLayer = heroParallaxSection.querySelector('[data-parallax-layer="base"]');
  const overlayLayer = heroParallaxSection.querySelector('[data-parallax-layer="overlay"]');
  let ticking = false;

  const updateHeroParallax = () => {
    ticking = false;

    const rect = heroParallaxSection.getBoundingClientRect();
    const viewportHeight = window.innerHeight || document.documentElement.clientHeight;
    const offsetFromCenter = rect.top + rect.height / 2 - viewportHeight / 2;

    if (baseLayer) {
      baseLayer.style.transform = `translateY(${offsetFromCenter * -0.14}px)`;
    }

    if (overlayLayer) {
      overlayLayer.style.transform = `translateY(${offsetFromCenter * -0.07}px)`;
    }
  };

  const requestHeroParallaxUpdate = () => {
    if (ticking) {
      return;
    }

    ticking = true;
    window.requestAnimationFrame(updateHeroParallax);
  };

  window.addEventListener("scroll", requestHeroParallaxUpdate, { passive: true });
  window.addEventListener("resize", requestHeroParallaxUpdate);
  requestHeroParallaxUpdate();
}

if (!reduceMotionPreference.matches) {
  let siteBackgroundTicking = false;

  const updateSiteBackgroundParallax = () => {
    siteBackgroundTicking = false;
    const farOffset = Math.round(window.scrollY * -0.08);
    const middleOffset = Math.round(window.scrollY * -promptForgeMiddleBackgroundRate);
    document.documentElement.style.setProperty("--site-bg-scroll", `${farOffset}px`);
    document.documentElement.style.setProperty("--site-mid-scroll", `${middleOffset}px`);
  };

  const requestSiteBackgroundParallaxUpdate = () => {
    if (siteBackgroundTicking) {
      return;
    }

    siteBackgroundTicking = true;
    window.requestAnimationFrame(updateSiteBackgroundParallax);
  };

  window.addEventListener("scroll", requestSiteBackgroundParallaxUpdate, { passive: true });
  window.addEventListener("resize", requestSiteBackgroundParallaxUpdate);
  requestSiteBackgroundParallaxUpdate();
}

const updateSiteBackgroundHeight = () => {
  const measuredHeight = measureSiteContentBottom();
  if (!measuredHeight) {
    return;
  }
  const paddedHeight = Math.ceil(measuredHeight + 40);

  document.documentElement.style.setProperty("--site-bg-height", `${paddedHeight}px`);
  syncSiteBackgroundMiddleStack();
};

window.addEventListener("resize", updateSiteBackgroundHeight);
window.addEventListener("load", updateSiteBackgroundHeight);
updateSiteBackgroundHeight();
activateExpandableImages();

document.documentElement.style.setProperty("--bg-panel-top-left", `url("${promptForgeBackgroundPanels.topLeft}")`);
document.documentElement.style.setProperty("--bg-panel-top-right", `url("${promptForgeBackgroundPanels.topRight}")`);
document.documentElement.style.setProperty("--bg-panel-bottom-left", `url("${promptForgeBackgroundPanels.bottomLeft}")`);
document.documentElement.style.setProperty("--bg-panel-bottom-right", `url("${promptForgeBackgroundPanels.bottomRight}")`);
